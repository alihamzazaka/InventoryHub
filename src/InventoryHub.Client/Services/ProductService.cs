using System.Net.Http.Json;
using InventoryHub.Client.Models;

namespace InventoryHub.Client.Services;

public sealed class ProductService(HttpClient http)
{
    public async Task<PagedResponse<Product>?> GetProductsAsync(int page = 1, int pageSize = 10, string? search = null, string? category = null, CancellationToken cancellationToken = default)
    {
        var url = $"api/products?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrWhiteSpace(category)) url += $"&category={Uri.EscapeDataString(category)}";
        return await http.GetFromJsonAsync<PagedResponse<Product>>(url, cancellationToken);
    }

    public async Task<Product?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await http.GetFromJsonAsync<ApiResponse<Product>>($"api/products/{id}", cancellationToken);
        return response?.Data;
    }
}
