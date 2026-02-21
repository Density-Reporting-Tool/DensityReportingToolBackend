namespace DensityReportingToolBackend.Mappings;

using AutoMapper;
using DensityReportingToolBackend.DTOs.Reports;
using DensityReportingToolBackend.Models;

public class ReportsMappingProfile : Profile
{
    public ReportsMappingProfile()
    {
        CreateMap<Report, ReportReadDto>();
        CreateMap<ReportPhoto, ReportPhotoReadDto>();
        CreateMap<ReportMemo, ReportMemoReadDto>();
        CreateMap<MemoComment, MemoCommentReadDto>();
    }
}