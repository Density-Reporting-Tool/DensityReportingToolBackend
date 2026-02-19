namespace DensityReportingToolBackend.Mappings;

using AutoMapper;
using DensityReportingToolBackend.DTOs.Labs;
using DensityReportingToolBackend.Models;

public class LabMappingProfile : Profile
{
    public LabMappingProfile()
    {
        CreateMap<Sieve, SieveReadDto>()
            .ForMember(dest => dest.ComputedRows, opt => opt.MapFrom(src => src.ComputePercentages()));
        
        CreateMap<SieveResult, SieveResultReadDto>();
        CreateMap<Proctor, ProctorReadDto>();
        CreateMap<ProctorType, ProctorTypeReadDto>();
        CreateMap<ProctorAdditionalJob, ProctorAdditionalJobReadDto>();
    }
}