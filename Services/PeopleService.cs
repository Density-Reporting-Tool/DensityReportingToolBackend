using AutoMapper;
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Infrastructure.Common;
using DensityReportingToolBackend.Infrastructure.Extensions;
using DensityReportingToolBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services;
public interface IPeopleService
{
    Task<IEnumerable<RoleReadDto>> GetAllRolesAsync();
    Task<PagedResult<PersonListFlatDto>> GetAllPeopleAsync(int pageNumber, int pageSize);
    Task<GeoPacificEmployeeFlatDto?> GetEmployeeByIdAsync(int id);
    Task<PersonalInfoReadDto?> GetContractorByIdAsync(int id);
    Task<GeoPacificEmployeeReadDto> CreateEmployeeAsync(GeoPacificEmployeeCreateDto dto);
    Task<PersonalInfoReadDto> CreateContractorAsync(PersonalInfoCreateDto dto);
    Task<GeoPacificEmployeeReadDto?> UpdateEmployeeAsync(int id, GeoPacificEmployeeUpdateDto dto);
    Task<PersonalInfoReadDto?> UpdateContractorAsync(int id, PersonalInfoUpdateDto dto);
}

public class PeopleService(AppDbContext dbContext, IMapper mapper) : IPeopleService
{
    public async Task<IEnumerable<RoleReadDto>> GetAllRolesAsync()
    {
        var roles = await dbContext.Roles
            .AsNoTracking()
            .OrderBy(r => r.RoleTitle)
            .ToListAsync();

        return mapper.Map<IEnumerable<RoleReadDto>>(roles);
    }

    public async Task<PagedResult<PersonListFlatDto>> GetAllPeopleAsync(int pageNumber, int pageSize)
    {
        var query = dbContext.PersonalInfos
            .Include(p => p.Employee)
                .ThenInclude(e => e!.Role)
            .AsNoTracking()
            .OrderBy(p => p.LastName);

        var pagedEntities = await query.ToPagedResultAsync(pageNumber, pageSize);

        var dtos = pagedEntities.Items.Select(p =>
        {
            var dto = mapper.Map<PersonListFlatDto>(p);
            dto.PersonType = p.Employee != null ? "GeoPacific Employee" : "Contact";
            dto.Role = p.Employee?.Role?.RoleTitle;
            return dto;
        }).ToList();

        return new PagedResult<PersonListFlatDto>(dtos, pagedEntities.Metadata);
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

    public async Task<PersonalInfoReadDto?> GetContractorByIdAsync(int id)
    {
        var person = await dbContext.PersonalInfos
            .Include(p => p.Employee)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.Employee == null);

        if (person == null) return null;

        return mapper.Map<PersonalInfoReadDto>(person);
    }

    public async Task<GeoPacificEmployeeReadDto> CreateEmployeeAsync(GeoPacificEmployeeCreateDto dto)
    {
        await ThrowIfEmailTakenAsync(dto.Email);

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
        await ThrowIfEmailTakenAsync(dto.Email);

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

    public async Task<GeoPacificEmployeeReadDto?> UpdateEmployeeAsync(int id, GeoPacificEmployeeUpdateDto dto)
    {
        var employee = await dbContext.GeoPacificEmployees
            .Include(e => e.PersonalInfo)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return null;

        if (!string.Equals(employee.PersonalInfo.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            await ThrowIfEmailTakenAsync(dto.Email);

        employee.PersonalInfo.FirstName = dto.FirstName;
        employee.PersonalInfo.LastName = dto.LastName;
        employee.PersonalInfo.Email = dto.Email;
        employee.PersonalInfo.PhoneNumber = dto.PhoneNumber;
        employee.RoleId = dto.RoleId;

        if (!string.IsNullOrEmpty(dto.Password))
            employee.Password = dto.Password;

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(employee).Reference(e => e.Role).LoadAsync();

        return mapper.Map<GeoPacificEmployeeReadDto>(employee);
    }

    public async Task<PersonalInfoReadDto?> UpdateContractorAsync(int id, PersonalInfoUpdateDto dto)
    {
        var person = await dbContext.PersonalInfos
            .FirstOrDefaultAsync(p => p.Id == id && p.Employee == null);

        if (person == null) return null;

        if (!string.Equals(person.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            await ThrowIfEmailTakenAsync(dto.Email);

        person.FirstName = dto.FirstName;
        person.LastName = dto.LastName;
        person.Email = dto.Email;
        person.PhoneNumber = dto.PhoneNumber;
        person.Company = dto.Company;

        await dbContext.SaveChangesAsync();

        return mapper.Map<PersonalInfoReadDto>(person);
    }

    private async Task ThrowIfEmailTakenAsync(string email)
    {
        var exists = await dbContext.PersonalInfos
            .AnyAsync(p => p.Email.ToLower() == email.ToLower());

        if (exists)
            throw new InvalidOperationException($"A person with email '{email}' is already registered.");
    }
}
