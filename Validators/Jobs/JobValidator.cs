using FluentValidation;
using DensityReportingToolBackend.DTOs.Jobs;

namespace DensityReportingToolBackend.Validators.Jobs;

public class JobCreateValidator : AbstractValidator<JobCreateDto>
{
    public JobCreateValidator()
    {
        RuleFor(x => x.JobNumber).NotEmpty().WithMessage("Job Number is required");
        RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Project Name is required");
        RuleFor(x => x.SiteAddress).NotEmpty().WithMessage("Site Address is required");
        RuleFor(x => x.ClientName).NotEmpty().WithMessage("Client Name is required");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date cannot be before start date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}

public class JobUpdateValidator : AbstractValidator<JobUpdateDto>
{
    public JobUpdateValidator()
    {
        // Ensure the ID is valid
        RuleFor(x => x.Id).GreaterThan(0);

        // Maintain the same date logic
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date cannot be before start date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}