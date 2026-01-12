using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Repositories;

public interface IJobRepository
{
    Task<IEnumerable<Job>> GetAllAsync();
    Task<Job?> GetByNumberAsync(string jobNumber);
    Task AddAsync(Job job);
    Task UpdateAsync(Job job);
    Task SaveChangesAsync();
}

public class JobRepository(AppDbContext dbContext) : IJobRepository
{
    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await dbContext.Jobs
            .Include(j => j.ProjectManagers).ThenInclude(pm => pm.PersonalInfo)
            .Include(j => j.SiteContacts).ThenInclude(sc => sc.PersonalInfo)
            .OrderByDescending(j => j.StartDate)
            .ToListAsync();
    }

    public async Task<Job?> GetByNumberAsync(string jobNumber)
    {
        return await dbContext.Jobs
            .Include(j => j.JobNotes)
            .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
    }

    public async Task AddAsync(Job job) => await dbContext.Jobs.AddAsync(job);
    
    public async Task UpdateAsync(Job job) => await Task.CompletedTask; // EF Change Tracking handles this

    public async Task SaveChangesAsync() => await dbContext.SaveChangesAsync();
}