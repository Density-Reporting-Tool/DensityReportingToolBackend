using Microsoft.EntityFrameworkCore;
using AutoMapper;

using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Jobs;
using DensityReportingToolBackend.Data;

namespace DensityReportingToolBackend.Services;

public interface IJobService
{
    Task<IEnumerable<JobReadDto>> ListJobsAsync();
    Task<JobReadDto> GetJobByNumberAsync(string jobNumber);
    Task<JobReadDto> CreateJobAsync(JobCreateDto dto);
    Task<JobReadDto> UpdateJobAsync(int id, JobUpdateDto dto);
}

public class JobService(AppDbContext dbContext, IMapper mapper) : IJobService
{
    public async Task<IEnumerable<JobReadDto>> ListJobsAsync()
    {
        var jobs = await dbContext.Jobs
            .Include(j => j.JobNotes)
            .Include(j => j.SitePlans)
                .ThenInclude(sp => sp.ShotPlacements)
            .Include(j => j.ProjectManagers)
                .ThenInclude(pm => pm.PersonalInfo)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<IEnumerable<JobReadDto>>(jobs);
    }

    public async Task<JobReadDto> GetJobByNumberAsync(string jobNumber)
    {
        var job = await dbContext.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
        return mapper.Map<JobReadDto>(job);
    }

    public async Task<JobReadDto> CreateJobAsync(JobCreateDto dto)
    {
        var job = mapper.Map<Job>(dto);
        await dbContext.AddAsync(job);
        await dbContext.SaveChangesAsync();
        return mapper.Map<JobReadDto>(job);
    }

    public async Task<JobReadDto> UpdateJobAsync(int id, JobUpdateDto dto)
    {
        var existingJob = await dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == dto.JobNumber) ?? throw new KeyNotFoundException();
        mapper.Map(dto, existingJob);

        await dbContext.SaveChangesAsync();
        return mapper.Map<JobReadDto>(existingJob);
    }
}