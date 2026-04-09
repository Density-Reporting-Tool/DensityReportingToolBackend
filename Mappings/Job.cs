namespace DensityReportingToolBackend.Mappings;

using AutoMapper;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Jobs;
using DensityReportingToolBackend.DTOs.People;

public class JobMappingProfile : Profile
{
    public JobMappingProfile()
    {
        CreateMap<Job, JobReadDto>();
        CreateMap<Job, JobRefDto>();
        CreateMap<JobCreateDto, Job>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<JobUpdateDto, Job>();

        CreateMap<JobProjectManager, JobProjectManagerReadDto>();
        CreateMap<JobSiteContact, JobSiteContactReadDto>();
        CreateMap<JobNote, JobNoteReadDto>();
        CreateMap<DistributionList, DistributionListReadDto>();
        CreateMap<DistributionMember, DistributionMemberReadDto>();
        CreateMap<SitePlan, SitePlanReadDto>();

        CreateMap<PersonalInfo, PersonalInfoReadDto>();
        CreateMap<GeoPacificEmployee, GeoPacificEmployeeReadDto>();
        CreateMap<Role, RoleReadDto>();
    }
}