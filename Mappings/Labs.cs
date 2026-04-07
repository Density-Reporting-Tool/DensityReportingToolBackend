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
        CreateMap<LabTest, LabTestReadDto>();
        CreateMap<Proctor, ProctorReadDto>();
        CreateMap<ProctorCreateDto, Proctor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DateSampled, opt => opt.MapFrom(src =>
                src.DateSampled.HasValue ? DateTime.SpecifyKind(src.DateSampled.Value, DateTimeKind.Utc) : (DateTime?)null))
            .ForMember(dest => dest.DateTested, opt => opt.MapFrom(src =>
                src.DateTested.HasValue ? DateTime.SpecifyKind(src.DateTested.Value, DateTimeKind.Utc) : (DateTime?)null));
        CreateMap<ProctorUpdateDto, Proctor>()
            .ForMember(dest => dest.DateSampled, opt => opt.MapFrom(src =>
                src.DateSampled.HasValue ? DateTime.SpecifyKind(src.DateSampled.Value, DateTimeKind.Utc) : (DateTime?)null))
            .ForMember(dest => dest.DateTested, opt => opt.MapFrom(src =>
                src.DateTested.HasValue ? DateTime.SpecifyKind(src.DateTested.Value, DateTimeKind.Utc) : (DateTime?)null));
        CreateMap<ProctorType, ProctorTypeReadDto>();
        CreateMap<ProctorAdditionalJob, ProctorAdditionalJobReadDto>();
    }
}