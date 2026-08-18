namespace InventoryHub.Client.Models;

public sealed class Product { public int Id { get; set; } public string Name { get; set; } = ""; public string Description { get; set; } = ""; public decimal Price { get; set; } public int Quantity { get; set; } public string Category { get; set; } = ""; public DateTime UpdatedAtUtc { get; set; } }
public sealed record ApiResponse<T>(T Data, string Message);
public sealed record PagedResponse<T>(IReadOnlyList<T> Data, int Page, int PageSize, int TotalCount);
