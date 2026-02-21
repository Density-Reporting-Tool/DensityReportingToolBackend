namespace DensityReportingToolBackend.Mappings;

using AutoMapper;
using DensityReportingToolBackend.DTOs.Density;
using DensityReportingToolBackend.Models;

public class DensityMappingProfile : Profile
{
    public DensityMappingProfile()
    {
        CreateMap<DensityTest, DensityTestReadDto>();
        CreateMap<DensityTestComment, DensityTestCommentReadDto>();
        CreateMap<ShotPlacement, ShotPlacementReadDto>();
    }
}