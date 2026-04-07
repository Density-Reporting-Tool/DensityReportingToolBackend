using Microsoft.EntityFrameworkCore;
using AutoMapper;

using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Calendar;
using DensityReportingToolBackend.Data;


namespace DensityReportingToolBackend.Services;



public interface ISchedulingService
{
    Task<ScheduleJobReadDto> CreateJobEventAsync(ScheduleJobCreateDto dto);
    Task<ScheduleJobReadDto> UpdateJobEventAsync(int id, ScheduleJobUpdateDto dto);
    Task<ScheduleJobReadDto> DeleteJobEventAsync(int id);   // or Task if you prefer
    Task<ScheduleJobReadDto> GetJobEventByIdAsync(int id);

    Task<IEnumerable<ScheduleJobReadDto>> GetEventsInRangeAsync(DateTimeOffset start, DateTimeOffset end);
}

public class SchedulingService(AppDbContext dbContext, IMapper mapper) : ISchedulingService
{
    public async Task<ScheduleJobReadDto> CreateJobEventAsync(ScheduleJobCreateDto dto)
    {
        var entity = mapper.Map<JobEvent>(dto);
        entity.CreatedDate = DateTimeOffset.UtcNow;

        dbContext.Set<JobEvent>().Add(entity);
        await dbContext.SaveChangesAsync();

        var withNavs = await dbContext.Set<JobEvent>()
            .Include(e => e.Job)
            .Include(e => e.PersonalInfo)
            .FirstAsync(e => e.Id == entity.Id);

        return mapper.Map<ScheduleJobReadDto>(withNavs);
    }

    public async Task<ScheduleJobReadDto> UpdateJobEventAsync(int id, ScheduleJobUpdateDto dto)
    {
        var entity = await dbContext.Set<JobEvent>()
            .Include(e => e.Job)
            .Include(e => e.PersonalInfo)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException();
        }

        mapper.Map(dto, entity);
        await dbContext.SaveChangesAsync();

        return mapper.Map<ScheduleJobReadDto>(entity);
    }

    public async Task<ScheduleJobReadDto> DeleteJobEventAsync(int id)
    {
        var entity = await dbContext.Set<JobEvent>()
            .Include(e => e.Job)
            .Include(e => e.PersonalInfo)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException();
        }

        dbContext.Set<JobEvent>().Remove(entity);
        await dbContext.SaveChangesAsync();

        return mapper.Map<ScheduleJobReadDto>(entity);
    }

    public async Task<ScheduleJobReadDto> GetJobEventByIdAsync(int id)
    {
        var entity = await dbContext.Set<JobEvent>()
            .Include(e => e.Job)
            .Include(e => e.PersonalInfo)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException();
        }

        return mapper.Map<ScheduleJobReadDto>(entity);
    }

    public async Task<IEnumerable<ScheduleJobReadDto>> GetEventsInRangeAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var entities = await dbContext.Set<JobEvent>()
            .Include(e => e.Job)
            .Include(e => e.PersonalInfo)
            .Where(e => e.StartDateTime >= start && e.EndDateTime <= end)
            .ToListAsync();

        return mapper.Map<IEnumerable<ScheduleJobReadDto>>(entities);
    }
}