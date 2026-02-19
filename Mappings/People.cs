using AutoMapper;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Models;


namespace DensityReportingToolBackend.Mappings;
public class PeopleMappingProfile : Profile
{
    public PeopleMappingProfile()
    {
        CreateMap<GeoPacificEmployee, GeoPacificEmployeeFlatDto>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalInfo.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.PersonalInfo.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalInfo.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalInfo.PhoneNumber))
            .ForMember(d => d.RoleTitle, o => o.MapFrom(s => s.Role.RoleTitle));

        CreateMap<PersonalInfo, PersonListFlatDto>();

        CreateMap<PersonalInfoCreateDto, PersonalInfo>();
        CreateMap<GeoPacificEmployeeCreateDto, PersonalInfo>()
            .ForMember(d => d.Company, opt => opt.Ignore());
    }
}