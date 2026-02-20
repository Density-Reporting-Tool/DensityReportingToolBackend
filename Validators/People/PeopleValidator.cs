using FluentValidation;
using DensityReportingToolBackend.DTOs.People;

namespace DensityReportingToolBackend.Validators.People;

public class EmployeeCreateValidator : AbstractValidator<GeoPacificEmployeeCreateDto>
{
    public EmployeeCreateValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.RoleId).GreaterThan(0);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class ContractorCreateValidator : AbstractValidator<PersonalInfoCreateDto>
{
    public ContractorCreateValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Company).NotEmpty();
    }
}
