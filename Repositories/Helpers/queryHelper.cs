using System.Linq.Expressions;
using DensityReportingToolBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Repositories.Helpers;

public class QueryHelper(AppDbContext dbContext)
{
    private readonly AppDbContext _context = dbContext;

    public async Task<T?> GetAsync<T>(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null) where T : class
    {
        IQueryable<T> query = _context.Set<T>();
        if (includeFunc != null) query = includeFunc(query);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null) where T : class
    {
        IQueryable<T> query = _context.Set<T>();
        if (includeFunc != null) query = includeFunc(query);
        return await query.ToListAsync();
    }
}