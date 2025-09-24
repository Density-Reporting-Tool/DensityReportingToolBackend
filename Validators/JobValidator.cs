using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.Validators;

public static class JobValidator
{
    public static ValidationResultDto Validate(JobCreateDto dto)
    {
        var result = new ValidationResultDto();

        if (string.IsNullOrWhiteSpace(dto.JobNumber))
            result.AddError(nameof(dto.JobNumber), "Job Number is required");

        if (string.IsNullOrWhiteSpace(dto.ProjectName))
            result.AddError(nameof(dto.ProjectName), "Project Name is required");

        if (string.IsNullOrWhiteSpace(dto.SiteAddress))
            result.AddError(nameof(dto.SiteAddress), "Site Address is required");

        if (string.IsNullOrWhiteSpace(dto.ClientName))
            result.AddError(nameof(dto.ClientName), "Client Name is required");

        if (dto.StartDate != null && dto.EndDate != null && dto.EndDate < dto.StartDate)
            result.AddError(nameof(dto.EndDate), "End date cannot be before start date");

        return result;
    }

    public static ValidationResultDto Validate(JobUpdateDto dto)
    {
        var result = new ValidationResultDto();

        if (dto.StartDate != null && dto.EndDate != null && dto.EndDate < dto.StartDate)
            result.AddError(nameof(dto.EndDate), "End date cannot be before start date");

        return result;
    }
}