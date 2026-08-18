using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Api.Models;

public sealed class ProductRequest
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Required, StringLength(60)]
    public string Category { get; set; } = string.Empty;
}
