using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Repositories.Helpers;
using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.Repositories;

public interface IPeopleRepository
{
    Task<IEnumerable<PersonalInfo>> GetAllPeopleAsync(Func<IQueryable<PersonalInfo>, IQueryable<PersonalInfo>>? includeFunc = null);
    Task<PersonalInfo?> GetPersonByIdAsync(int id, Func<IQueryable<PersonalInfo>, IQueryable<PersonalInfo>>? includeFunc = null);
    Task<GeoPacificEmployee?> GetEmployeeByIdAsync(int id, Func<IQueryable<GeoPacificEmployee>, IQueryable<GeoPacificEmployee>>? includeFunc = null);

    Task AddPersonAsync(PersonalInfo person);
    Task AddEmployeeAsync(GeoPacificEmployee employee);
    Task SaveChangesAsync();
}

public class PeopleRepository(AppDbContext dbContext) : IPeopleRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly QueryHelper _queryHelper = new(dbContext);

    public async Task<IEnumerable<PersonalInfo>> GetAllPeopleAsync(Func<IQueryable<PersonalInfo>, IQueryable<PersonalInfo>>? includeFunc = null)
    {
        return await _queryHelper.GetAllAsync(includeFunc);
    }

    public async Task<PersonalInfo?> GetPersonByIdAsync(int id, Func<IQueryable<PersonalInfo>, IQueryable<PersonalInfo>>? includeFunc = null)
    {
        return await _queryHelper.GetAsync<PersonalInfo>(p => p.Id == id, includeFunc);
    }

    public async Task<GeoPacificEmployee?> GetEmployeeByIdAsync(int id, Func<IQueryable<GeoPacificEmployee>, IQueryable<GeoPacificEmployee>>? includeFunc = null)
    {
        return await _queryHelper.GetAsync<GeoPacificEmployee>(e => e.Id == id, includeFunc);
    }

    public async Task AddPersonAsync(PersonalInfo person) 
        => await _dbContext.PersonalInfos.AddAsync(person);

    public async Task AddEmployeeAsync(GeoPacificEmployee employee) 
        => await _dbContext.GeoPacificEmployees.AddAsync(employee);

    public async Task SaveChangesAsync() 
        => await _dbContext.SaveChangesAsync();
}