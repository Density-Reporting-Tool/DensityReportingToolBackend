using Microsoft.EntityFrameworkCore;
using DensityReportingToolBackend.Infrastructure.Common;

namespace DensityReportingToolBackend.Infrastructure.Extensions;

public static class PaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();
        
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        var metadata = new PagedMetadata(
            CurrentPage: pageNumber,
            PageSize: pageSize,
            TotalCount: count,
            TotalPages: totalPages,
            HasNextPage: pageNumber < totalPages,
            HasPreviousPage: pageNumber > 1
        );

        return new PagedResult<T>(items, metadata);
    }
}