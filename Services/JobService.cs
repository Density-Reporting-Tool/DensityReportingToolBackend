using Microsoft.EntityFrameworkCore;
using DensityReportingToolBackend.Repositories;
using AutoMapper;

using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Jobs;

namespace DensityReportingToolBackend.Services;

public interface IJobService
{
    Task<IEnumerable<JobReadDto>> ListJobsAsync();
    Task<JobReadDto> GetJobByNumberAsync(string jobNumber);
    Task<JobReadDto> CreateJobAsync(JobCreateDto dto);
    Task<JobReadDto> UpdateJobAsync(int id, JobUpdateDto dto);
}

public class JobService(IJobRepository repository, IMapper mapper) : IJobService
{
    public async Task<IEnumerable<JobReadDto>> ListJobsAsync()
    {
        var jobs = await repository.GetAllAsync(query => query
        .Include(j => j.JobNotes)
        .Include(j => j.SitePlans)
            .ThenInclude(sp => sp.ShotPlacements)
        .Include(j => j.ProjectManagers)
            .ThenInclude(pm => pm.PersonalInfo)
        );
        return mapper.Map<IEnumerable<JobReadDto>>(jobs);
    }

    public async Task<JobReadDto> GetJobByNumberAsync(string jobNumber)
    {
        Job? job = await repository.GetByNumberAsync(jobNumber);
        return mapper.Map<JobReadDto>(job);
    }

    public async Task<JobReadDto> CreateJobAsync(JobCreateDto dto)
    {
        var job = mapper.Map<Job>(dto);
        await repository.AddAsync(job);
        await repository.SaveChangesAsync();
        return mapper.Map<JobReadDto>(job);
    }

    public async Task<JobReadDto> UpdateJobAsync(int id, JobUpdateDto dto)
    {
        var existingJob = await repository.GetByNumberAsync(dto.JobNumber) ?? throw new KeyNotFoundException();
        mapper.Map(dto, existingJob);

        await repository.SaveChangesAsync();
        return mapper.Map<JobReadDto>(existingJob);
    }
}