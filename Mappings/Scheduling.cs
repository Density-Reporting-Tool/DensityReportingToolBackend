using AutoMapper;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Calendar;
using DensityReportingToolBackend.DTOs.Jobs;
using DensityReportingToolBackend.DTOs.People;


namespace DensityReportingToolBackend.Mappings;
public class SchedulingMappingProfile : Profile
{
    public SchedulingMappingProfile()
    {
        // Entity -> Read DTO
        CreateMap<JobEvent, ScheduleJobReadDto>()
            .ForMember(d => d.Job, opt => opt.MapFrom(src => src.Job))
            .ForMember(d => d.PersonalInfo, opt => opt.MapFrom(src => src.PersonalInfo));

        // Create DTO -> Entity
        CreateMap<ScheduleJobCreateDto, JobEvent>()
            .ForMember(d => d.Id, opt => opt.Ignore())        // DB generates
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.MapFrom(_ => "Scheduled"));

        // Update DTO -> Entity
        CreateMap<ScheduleJobUpdateDto, JobEvent>()
            .ForMember(d => d.Id, opt => opt.Ignore())        // use route id instead
            .ForMember(d => d.CreatedDate, opt => opt.Ignore());
    }
}