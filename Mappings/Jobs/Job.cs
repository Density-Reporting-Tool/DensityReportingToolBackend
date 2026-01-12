namespace DensityReportingToolBackend.Mappings.Jobs;

using AutoMapper;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Jobs;

public class JobMappingProfile : Profile
{
    public JobMappingProfile()
    {
        // Entity -> DTO (For Reading)
        CreateMap<Job, JobReadDto>();

        // DTO -> Entity (For Creating)
        // We ignore the Id property because the Database will auto-generate it
        CreateMap<JobCreateDto, Job>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // DTO -> Entity (For Updating)
        // This maps the DTO values onto an existing Entity from the DB
        CreateMap<JobUpdateDto, Job>();
    }
}