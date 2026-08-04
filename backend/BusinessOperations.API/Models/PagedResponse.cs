namespace BusinessOperations.API.Models;

public sealed class PagedResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public PaginationMetadata Metadata { get; set; }

    public PagedResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize, string message = "Data retrieved successfully.")
    {
        Data = items;
        Message = message;
        Metadata = new PaginationMetadata(totalCount, pageNumber, pageSize);
    }
}
