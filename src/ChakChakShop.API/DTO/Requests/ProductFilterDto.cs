namespace ChakChakShop.API.DTO.Requests;

public class ProductFilterDto : PagedRequest
{
    public string? Name { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
}


