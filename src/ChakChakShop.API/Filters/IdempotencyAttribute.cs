using Microsoft.AspNetCore.Mvc.Filters;

namespace ChakChakShop.API.Filters;

public class IdempotencyAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<Services.ICacheService>();
        var idempotencyKey = context.HttpContext.Request.Headers["Idempotency-Key"].FirstOrDefault();

        if (string.IsNullOrEmpty(idempotencyKey))
        {
            context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                new { Message = "Idempotency-Key header is required" });
            return;
        }

        var cacheKey = $"idempotency_{idempotencyKey}";
        var cachedResponse = await cacheService.GetAsync<object>(cacheKey);

        if (cachedResponse != null)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.OkObjectResult(cachedResponse);
            return;
        }

        var executedContext = await next();

        if (executedContext.Result is Microsoft.AspNetCore.Mvc.ObjectResult result && result.Value != null)
        {
            await cacheService.SetAsync(cacheKey, result.Value, TimeSpan.FromHours(24));
        }
    }
}


