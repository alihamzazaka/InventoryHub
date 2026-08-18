namespace InventoryHub.Api.Models;

public sealed record ApiResponse<T>(T Data, string Message = "Success");
public sealed record PagedResponse<T>(IReadOnlyList<T> Data, int Page, int PageSize, int TotalCount);
public sealed record ErrorResponse(string Error, string Code, string? Details = null);
