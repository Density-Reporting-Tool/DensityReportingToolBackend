using Microsoft.EntityFrameworkCore;
using AutoMapper;

using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Labs;
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Infrastructure.Common;
using DensityReportingToolBackend.Infrastructure.Extensions;

namespace DensityReportingToolBackend.Services;

public interface IProctorService
{
    Task<PagedResult<ProctorReadDto>> ListProctorsAsync(int pageNumber, int pageSize);
    Task<IEnumerable<ProctorReadDto>> SearchProctorsByJobNumberAsync(string jobNumber, int limit);
    Task<ProctorReadDto?> GetProctorByIdAsync(int id);
    Task<IEnumerable<ProctorReadDto>> GetProctorsForJobAsync(string jobNumber);
    Task<IEnumerable<ProctorReadDto>> GetProctorsForJobByIdAsync(int jobId);
    Task<ProctorReadDto> CreateProctorAsync(ProctorCreateDto dto);
    Task<ProctorReadDto> UpdateProctorAsync(int id, ProctorUpdateDto dto);
}

public class ProctorService(AppDbContext dbContext, IMapper mapper) : IProctorService
{
    private IQueryable<Proctor> ProctorsWithIncludes() =>
        dbContext.Proctors
            .Include(p => p.LabTest)
                .ThenInclude(lt => lt.Job)
            .Include(p => p.ProctorType)
            .Include(p => p.Sieve)
            .Include(p => p.AdditionalJobs)
                .ThenInclude(paj => paj.Job)
            .Include(p => p.DensityTests);

    public async Task<PagedResult<ProctorReadDto>> ListProctorsAsync(int pageNumber, int pageSize)
    {
        var query = ProctorsWithIncludes()
            .AsNoTracking()
            .OrderByDescending(p => p.DateTested)
            .ThenBy(p => p.Id);

        var pagedEntities = await query.ToPagedResultAsync(pageNumber, pageSize);
        var dtos = mapper.Map<IEnumerable<ProctorReadDto>>(pagedEntities.Items);
        return new PagedResult<ProctorReadDto>(dtos, pagedEntities.Metadata);
    }

    public async Task<ProctorReadDto?> GetProctorByIdAsync(int id)
    {
        var proctor = await ProctorsWithIncludes()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return proctor == null ? null : mapper.Map<ProctorReadDto>(proctor);
    }

    public async Task<IEnumerable<ProctorReadDto>> GetProctorsForJobAsync(string jobNumber)
    {
        var proctors = await ProctorsWithIncludes()
            .AsNoTracking()
            .Where(p => p.LabTest.Job.JobNumber == jobNumber)
            .OrderByDescending(p => p.DateTested)
            .ThenBy(p => p.Id)
            .ToListAsync();

        return mapper.Map<IEnumerable<ProctorReadDto>>(proctors);
    }

    public async Task<IEnumerable<ProctorReadDto>> GetProctorsForJobByIdAsync(int jobId)
    {
        var proctors = await ProctorsWithIncludes()
            .AsNoTracking()
            .Where(p => p.LabTest.JobId == jobId)
            .OrderByDescending(p => p.DateTested)
            .ThenBy(p => p.Id)
            .ToListAsync();

        return mapper.Map<IEnumerable<ProctorReadDto>>(proctors);
    }

    public async Task<IEnumerable<ProctorReadDto>> SearchProctorsByJobNumberAsync(string jobNumber, int limit)
    {
        var jobIds = await dbContext.Jobs
            .Where(j => j.JobNumber.Contains(jobNumber))
            .Select(j => j.Id)
            .ToListAsync();

        if (jobIds.Count == 0)
            return [];

        var proctors = await ProctorsWithIncludes()
            .AsNoTracking()
            .Where(p => jobIds.Contains(p.LabTest.JobId))
            .OrderByDescending(p => p.DateTested)
            .ThenBy(p => p.Id)
            .Take(limit)
            .ToListAsync();

        return mapper.Map<IEnumerable<ProctorReadDto>>(proctors);
    }

    public async Task<ProctorReadDto> CreateProctorAsync(ProctorCreateDto dto)
    {
        var labTestExists = await dbContext.LabTests.AnyAsync(lt => lt.Id == dto.LabTestId);
        if (!labTestExists)
            throw new KeyNotFoundException($"LabTest with ID {dto.LabTestId} not found.");

        var proctor = mapper.Map<Proctor>(dto);
        await dbContext.AddAsync(proctor);
        await dbContext.SaveChangesAsync();

        await dbContext.Entry(proctor).Reference(p => p.LabTest).LoadAsync();
        await dbContext.Entry(proctor.LabTest).Reference(lt => lt.Job).LoadAsync();
        await dbContext.Entry(proctor).Reference(p => p.ProctorType).LoadAsync();

        return mapper.Map<ProctorReadDto>(proctor);
    }

    public async Task<ProctorReadDto> UpdateProctorAsync(int id, ProctorUpdateDto dto)
    {
        var proctor = await dbContext.Proctors.FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Proctor with ID {id} not found.");

        mapper.Map(dto, proctor);
        await dbContext.SaveChangesAsync();

        await dbContext.Entry(proctor).Reference(p => p.LabTest).LoadAsync();
        await dbContext.Entry(proctor.LabTest).Reference(lt => lt.Job).LoadAsync();
        await dbContext.Entry(proctor).Reference(p => p.ProctorType).LoadAsync();

        return mapper.Map<ProctorReadDto>(proctor);
    }
}
