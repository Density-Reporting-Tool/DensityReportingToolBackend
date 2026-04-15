using AutoMapper;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Models;


namespace DensityReportingToolBackend.Mappings;
public class PeopleMappingProfile : Profile
{
    public PeopleMappingProfile()
    {
        // Entity → Read DTOs
        CreateMap<GeoPacificEmployee, GeoPacificEmployeeFlatDto>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalInfo.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.PersonalInfo.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalInfo.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalInfo.PhoneNumber))
            .ForMember(d => d.RoleTitle, o => o.MapFrom(s => s.Role.RoleTitle));

        CreateMap<PersonalInfo, PersonListFlatDto>();
        CreateMap<PersonalInfo, PersonalInfoReadDto>();
        CreateMap<GeoPacificEmployee, GeoPacificEmployeeReadDto>();
        CreateMap<Role, RoleReadDto>();

        // Create DTOs → Entity
        CreateMap<PersonalInfoCreateDto, PersonalInfo>();
        CreateMap<GeoPacificEmployeeCreateDto, PersonalInfo>()
            .ForMember(d => d.Company, opt => opt.Ignore());

        CreateMap<RoleCreateDto, Role>();

        // Update DTOs → Entity (Id ignored to prevent overwriting the PK)
        CreateMap<PersonalInfoUpdateDto, PersonalInfo>()
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<GeoPacificEmployeeUpdateDto, PersonalInfo>()
            .ForMember(d => d.Company, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<RoleUpdateDto, Role>()
            .ForMember(d => d.Id, opt => opt.Ignore());
    }
}
