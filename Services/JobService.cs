using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;

using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services;

public class JobService(AppDbContext dbContext)
{
    private readonly AppDbContext dbContext = dbContext;

    public async Task<IEnumerable<Job>> ListJobs()
    {
        return await dbContext.Jobs
                    .Include(j => j.ProjectManagers)
                        .ThenInclude(jpm => jpm.PersonalInfo)
                    .Include(j => j.SiteContacts)
                        .ThenInclude(jsc => jsc.PersonalInfo)
                    .OrderByDescending(j => j.StartDate)
                    .ThenBy(j => j.JobNumber)
                    .ToListAsync();
    }

    public async Task<Job?> GetJobByNumber(string jobNumber)
    {
        return await dbContext.Jobs
                    .Include(j => j.ProjectManagers)
                        .ThenInclude(jpm => jpm.PersonalInfo)
                    .Include(j => j.SiteContacts)
                        .ThenInclude(jsc => jsc.PersonalInfo)
                    .Include(j => j.DistributionLists)
                        .ThenInclude(dl => dl.DistributionMembers)
                            .ThenInclude(dm => dm.PersonalInfo)
                    .Include(j => j.Reports)
                    .Include(j => j.JobNotes)
                    .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
    }
}
