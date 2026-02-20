namespace DensityReportingToolBackend.Infrastructure.Common;

public record PagedMetadata(
    int CurrentPage, 
    int PageSize, 
    int TotalCount, 
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);

public record PagedResult<T>(IEnumerable<T> Items, PagedMetadata Metadata);