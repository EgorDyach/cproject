FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files first for better layer caching
COPY ["src/ChakChakShop.API/ChakChakShop.API.csproj", "src/ChakChakShop.API/"]

# Restore dependencies
RUN dotnet restore "src/ChakChakShop.API/ChakChakShop.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/ChakChakShop.API"
RUN dotnet build "ChakChakShop.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChakChakShop.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChakChakShop.API.dll"]

