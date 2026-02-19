namespace DensityReportingToolBackend.Mappings;

using AutoMapper;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.DTOs.Jobs;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.DTOs.Density;
using DensityReportingToolBackend.DTOs.Reports;
using DensityReportingToolBackend.DTOs.Labs;

public class JobMappingProfile : Profile
{
    public JobMappingProfile()
    {
        // 1. Core Job Mappings
        CreateMap<Job, JobReadDto>();
        CreateMap<JobCreateDto, Job>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<JobUpdateDto, Job>();

        // 2. Job Sub-Collections
        CreateMap<JobProjectManager, JobProjectManagerReadDto>();
        CreateMap<JobSiteContact, JobSiteContactReadDto>();
        CreateMap<JobNote, JobNoteReadDto>();
        CreateMap<DistributionList, DistributionListReadDto>();
        CreateMap<DistributionMember, DistributionMemberReadDto>();
        CreateMap<SitePlan, SitePlanReadDto>();

        // 3. People / PersonalInfo (Required for ProjectManagers and SiteContacts)
        CreateMap<PersonalInfo, PersonalInfoReadDto>();
        CreateMap<GeoPacificEmployee, GeoPacificEmployeeReadDto>();
        CreateMap<Role, RoleReadDto>();

        // 4. Reports and Memos
        CreateMap<Report, ReportReadDto>();
        CreateMap<ReportPhoto, ReportPhotoReadDto>();
        CreateMap<ReportMemo, ReportMemoReadDto>();
        CreateMap<MemoComment, MemoCommentReadDto>();

        // 5. Density and Labs
        CreateMap<DensityTest, DensityTestReadDto>();
        CreateMap<DensityTestComment, DensityTestCommentReadDto>();
        CreateMap<ShotPlacement, ShotPlacementReadDto>();
        
        // Use the custom logic for Sieve computed properties
        CreateMap<Sieve, SieveReadDto>()
            .ForMember(dest => dest.ComputedRows, opt => opt.MapFrom(src => src.ComputePercentages()));
        
        CreateMap<SieveResult, SieveResultReadDto>();
        CreateMap<Proctor, ProctorReadDto>();
        CreateMap<ProctorType, ProctorTypeReadDto>();
        CreateMap<ProctorAdditionalJob, ProctorAdditionalJobReadDto>();
    }
}