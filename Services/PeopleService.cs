using AutoMapper;
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services;
public interface IPeopleService
{
    Task<GeoPacificEmployeeReadDto> CreateEmployeeAsync(GeoPacificEmployeeCreateDto dto);
    Task<PersonalInfoReadDto> CreateContractorAsync(PersonalInfoCreateDto dto);
    Task<GeoPacificEmployeeFlatDto?> GetEmployeeByIdAsync(int id);
    Task<IEnumerable<PersonListFlatDto>> GetAllPeopleAsync();
}

public class PeopleService(AppDbContext dbContext, IMapper mapper) : IPeopleService
{
    public async Task<GeoPacificEmployeeReadDto> CreateEmployeeAsync(GeoPacificEmployeeCreateDto dto)
    {
        var employee = new GeoPacificEmployee
        {
            PersonalInfo = new PersonalInfo
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            },
            RoleId = dto.RoleId,
            Password = dto.Password
        };

        await dbContext.GeoPacificEmployees.AddAsync(employee);
        await dbContext.SaveChangesAsync();

        return mapper.Map<GeoPacificEmployeeReadDto>(employee);
    }

    public async Task<PersonalInfoReadDto> CreateContractorAsync(PersonalInfoCreateDto dto)
    {
        var personalInfo = new PersonalInfo
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Company = dto.Company
        };

        await dbContext.PersonalInfos.AddAsync(personalInfo);
        await dbContext.SaveChangesAsync();

        return mapper.Map<PersonalInfoReadDto>(personalInfo);
    }

    public async Task<IEnumerable<PersonListFlatDto>> GetAllPeopleAsync()
    {
        var people = await dbContext.PersonalInfos
            .Include(p => p.Employee)
                .ThenInclude(e => e!.Role)
            .AsNoTracking()
            .ToListAsync();

        var dtos = mapper.Map<IEnumerable<PersonListFlatDto>>(people);

        foreach (var dto in dtos)
        {
            var source = people.First(p => p.Id == dto.Id);
            
            if (source.Employee != null)
            {
                dto.PersonType = "GeoPacific Employee";
                dto.Role = source.Employee.Role?.RoleTitle;
            }
            else
            {
                dto.PersonType = "Contact";
                dto.Role = null;
            }
        }

        return dtos;
    }

    public async Task<GeoPacificEmployeeFlatDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await dbContext.GeoPacificEmployees
            .Include(e => e.PersonalInfo)
            .Include(e => e.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return null;

        return mapper.Map<GeoPacificEmployeeFlatDto>(employee);
    }
}