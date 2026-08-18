using InventoryHub.Api.Models;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
builder.Services.AddCors(options => options.AddPolicy("BlazorClient", policy => policy.WithOrigins("https://localhost:7001", "http://localhost:7001").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("BlazorClient");

var products = new List<Product>
{
    new() { Id=1, Name="Wireless Keyboard", Description="Compact mechanical keyboard", Price=59.99m, Quantity=42, Category="Electronics", UpdatedAtUtc=DateTime.UtcNow },
    new() { Id=2, Name="USB-C Hub", Description="7-in-1 USB-C connectivity hub", Price=34.50m, Quantity=75, Category="Electronics", UpdatedAtUtc=DateTime.UtcNow },
    new() { Id=3, Name="Office Chair", Description="Ergonomic adjustable chair", Price=189.00m, Quantity=18, Category="Furniture", UpdatedAtUtc=DateTime.UtcNow },
    new() { Id=4, Name="Standing Desk", Description="Electric height-adjustable desk", Price=399.00m, Quantity=11, Category="Furniture", UpdatedAtUtc=DateTime.UtcNow },
    new() { Id=5, Name="Notebook", Description="Hardcover dotted notebook", Price=8.99m, Quantity=120, Category="Stationery", UpdatedAtUtc=DateTime.UtcNow }
};

var group = app.MapGroup("/api/products").WithTags("Products");
group.MapGet("", (IMemoryCache cache, int page = 1, int pageSize = 10, string? search = null, string? category = null) =>
{
    page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100);
    var key = $"products:{page}:{pageSize}:{search?.Trim().ToLowerInvariant()}:{category?.Trim().ToLowerInvariant()}";
    var result = cache.GetOrCreate(key, entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);
        var query = products.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || p.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        var total = query.Count();
        return new PagedResponse<Product>(query.OrderBy(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList(), page, pageSize, total);
    });
    return Results.Ok(result);
});
group.MapGet("/{id:int}", (int id) => { var product = products.FirstOrDefault(p => p.Id == id); return product is null ? Results.NotFound(new ErrorResponse("Product not found", "PRODUCT_NOT_FOUND", $"No product exists with id {id}.")) : Results.Ok(new ApiResponse<Product>(product)); });
group.MapPost("", (ProductRequest request, IMemoryCache cache) => { var product = new Product { Id = products.Count == 0 ? 1 : products.Max(p => p.Id) + 1, Name = request.Name.Trim(), Description = request.Description.Trim(), Price = request.Price, Quantity = request.Quantity, Category = request.Category.Trim(), UpdatedAtUtc = DateTime.UtcNow }; products.Add(product); cache.Compact(1); return Results.Created($"/api/products/{product.Id}", new ApiResponse<Product>(product, "Product created")); });
group.MapPut("/{id:int}", (int id, ProductRequest request, IMemoryCache cache) => { var product = products.FirstOrDefault(p => p.Id == id); if (product is null) return Results.NotFound(new ErrorResponse("Product not found", "PRODUCT_NOT_FOUND")); product.Name=request.Name.Trim(); product.Description=request.Description.Trim(); product.Price=request.Price; product.Quantity=request.Quantity; product.Category=request.Category.Trim(); product.UpdatedAtUtc=DateTime.UtcNow; cache.Compact(1); return Results.Ok(new ApiResponse<Product>(product, "Product updated")); });
group.MapDelete("/{id:int}", (int id, IMemoryCache cache) => { var product=products.FirstOrDefault(p=>p.Id==id); if(product is null) return Results.NotFound(new ErrorResponse("Product not found", "PRODUCT_NOT_FOUND")); products.Remove(product); cache.Compact(1); return Results.NoContent(); });
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.Run();
