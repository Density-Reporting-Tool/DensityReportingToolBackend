using FluentValidation;
using DensityReportingToolBackend.DTOs.Labs;

namespace DensityReportingToolBackend.Validators.Labs;

public class ProctorBaseValidator<T> : AbstractValidator<T> where T : ProctorBaseDto
{
    public ProctorBaseValidator()
    {
        RuleFor(x => x.ProctorID)
            .NotEmpty().WithMessage("Proctor ID is required");

        RuleFor(x => x.MaterialType)
            .NotEmpty().WithMessage("Material Type is required");

        RuleFor(x => x.LabLocation)
            .NotEmpty().WithMessage("Lab Location is required");

        RuleFor(x => x.ProctorType)
            .NotEmpty().WithMessage("Proctor Type is required")
            .Must(x => x.Type == "Modified" || x.Type == "Standard")
            .WithMessage("Proctor Type must be 'Modified' or 'Standard'");

        RuleFor(x => x.MaxDensity)
            .InclusiveBetween(0, 3000)
            .When(x => x.MaxDensity.HasValue)
            .WithMessage("Max Density must be between 0 and 3000 kg/m³");

        RuleFor(x => x.CorrectedDensity)
            .InclusiveBetween(0, 3000)
            .When(x => x.CorrectedDensity.HasValue)
            .WithMessage("Corrected Density must be between 0 and 3000 kg/m³");

        RuleFor(x => x.OptimumMoistureContent)
            .InclusiveBetween(0, 100)
            .When(x => x.OptimumMoistureContent.HasValue)
            .WithMessage("Optimum Moisture must be between 0 and 100%");

        RuleFor(x => x.OversizePercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.OversizePercentage.HasValue)
            .WithMessage("Oversize Percentage must be between 0 and 100%");

        RuleFor(x => x.SpecificGravity)
            .InclusiveBetween(0, 10)
            .When(x => x.SpecificGravity.HasValue)
            .WithMessage("Specific Gravity must be between 0 and 10");

        RuleFor(x => x.DateTested)
            .GreaterThanOrEqualTo(x => x.DateSampled)
            .When(x => x.DateSampled.HasValue && x.DateTested.HasValue)
            .WithMessage("Date Tested cannot be before Date Sampled");
    }
}

public class ProctorCreateValidator : ProctorBaseValidator<ProctorCreateDto>
{
    public ProctorCreateValidator()
    {
        RuleFor(x => x.LabTestId)
            .GreaterThan(0).WithMessage("Proctor must be associated with a valid Lab Test.");
    }
}

public class ProctorUpdateValidator : ProctorBaseValidator<ProctorUpdateDto>
{
    public ProctorUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("A valid ID is required for updates")
            .GreaterThan(0).WithMessage("A valid ID is required for updates");
    }
}