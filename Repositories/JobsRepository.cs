using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Repositories;
using DensityReportingToolBackend.Repositories.Helpers;

public interface IJobRepository
{
    Task<IEnumerable<Job>> GetAllAsync(Func<IQueryable<Job>, IQueryable<Job>>? includeFunc = null);
    Task<Job?> GetByNumberAsync(string jobNumber, Func<IQueryable<Job>, IQueryable<Job>>? includeFunc = null);
    
    // Example of a tightly related model served by the same repo
    Task<DistributionList?> GetDistributionListAsync(int id, Func<IQueryable<DistributionList>, IQueryable<DistributionList>>? includeFunc = null);

    Task AddAsync(Job job);
    Task SaveChangesAsync();
}

public class JobRepository : IJobRepository
{
    private readonly AppDbContext _dbContext;
    private readonly QueryHelper _queryHelper;

    public JobRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _queryHelper = new QueryHelper(dbContext);
    }

    public async Task<IEnumerable<Job>> GetAllAsync(Func<IQueryable<Job>, IQueryable<Job>>? includeFunc = null)
    {

        return await _queryHelper.GetAllAsync(includeFunc);
    }

    public async Task<Job?> GetByNumberAsync(string jobNumber, Func<IQueryable<Job>, IQueryable<Job>>? includeFunc = null)
    {
        return await _queryHelper.GetAsync(
            j => j.JobNumber == jobNumber, 
            includeFunc);
    }

    public async Task<DistributionList?> GetDistributionListAsync(int id, Func<IQueryable<DistributionList>, IQueryable<DistributionList>>? includeFunc = null)
    {
        return await _queryHelper.GetAsync(
            d => d.Id == id, 
            includeFunc);
    }

    public async Task AddAsync(Job job) => await _dbContext.Jobs.AddAsync(job);

    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}