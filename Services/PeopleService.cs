using AutoMapper;
using DensityReportingToolBackend.Controllers;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services;
public interface IPeopleService
{
    Task<GeoPacificEmployeeReadDto> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<PersonalInfoReadDto> CreateContractorAsync(CreateContractorRequest request);
    Task<GeoPacificEmployeeFlatDto?> GetEmployeeByIdAsync(int id);
    Task<IEnumerable<PersonListFlatDto>> GetAllPeopleAsync();
}

public class PeopleService(IPeopleRepository peopleRepository, IMapper mapper) : IPeopleService
{
    public async Task<GeoPacificEmployeeReadDto> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        var employee = new GeoPacificEmployee
        {
            PersonalInfo = new PersonalInfo
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            },
            RoleId = request.RoleId,
            Password = request.Password
        };

        await peopleRepository.AddEmployeeAsync(employee);
        await peopleRepository.SaveChangesAsync();

        return mapper.Map<GeoPacificEmployeeReadDto>(employee);
    }

    public async Task<PersonalInfoReadDto> CreateContractorAsync(CreateContractorRequest request)
    {
        var personalInfo = new PersonalInfo
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Company = request.Company
        };

        await peopleRepository.AddPersonAsync(personalInfo);
        await peopleRepository.SaveChangesAsync();

        return mapper.Map<PersonalInfoReadDto>(personalInfo);
    }

    public async Task<IEnumerable<PersonListFlatDto>> GetAllPeopleAsync()
{
    var people = await peopleRepository.GetAllPeopleAsync(query => query
        .Include(p => p.Employee)
            .ThenInclude(e => e!.Role));

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
    var employee = await peopleRepository.GetEmployeeByIdAsync(id, query => query
        .Include(e => e.PersonalInfo)
        .Include(e => e.Role));

    if (employee == null) return null;

    return mapper.Map<GeoPacificEmployeeFlatDto>(employee);
}
}