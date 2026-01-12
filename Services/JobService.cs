using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Jobs;

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

    public async Task<IEnumerable<Job>> SearchJobsByJobNumber(string jobNumber, int limit = 10)
    {
        return await dbContext.Jobs
            .Where(j => j.JobNumber.Contains(jobNumber))
            .OrderBy(j => j.JobNumber)
            .Take(limit)
            .ToListAsync();
    }

public async Task<Job> CreateJob(JobCreateDto dto)
{
    var job = new Job
    {
        JobNumber = dto.JobNumber,
        ClientName = dto.ClientName,
        ProjectName = dto.ProjectName,
        SiteAddress = dto.SiteAddress,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate
    };

    dbContext.Jobs.Add(job);
    await dbContext.SaveChangesAsync();

    return job;
}

public async Task<Job> UpdateJob(int jobId, JobUpdateDto dto)
{
    var job = await dbContext.Jobs
        .Include(j => j.JobNotes)
        .FirstOrDefaultAsync(j => j.Id == jobId);

    if (job == null)
        throw new KeyNotFoundException($"Job with ID {jobId} not found.");

    if (!string.IsNullOrWhiteSpace(dto.JobNumber))
        job.JobNumber = dto.JobNumber;

    if (!string.IsNullOrWhiteSpace(dto.ClientName))
        job.ClientName = dto.ClientName;

    if (!string.IsNullOrWhiteSpace(dto.ProjectName))
        job.ProjectName = dto.ProjectName;

    if (!string.IsNullOrWhiteSpace(dto.SiteAddress))
        job.SiteAddress = dto.SiteAddress;

    if (dto.StartDate.HasValue)
        job.StartDate = dto.StartDate;

    if (dto.EndDate.HasValue)
        job.EndDate = dto.EndDate;

    await dbContext.SaveChangesAsync();
    return job;
}
}
