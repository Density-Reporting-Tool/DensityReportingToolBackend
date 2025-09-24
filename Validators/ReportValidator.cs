using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.Validators;

public static class ReportValidator
{
    public static ValidationResultDto Validate(ReportCreateDto dto)
    {
        var result = new ValidationResultDto();

        if (dto.JobId <= 0)
            result.AddError(nameof(dto.JobId), "Valid Job ID is required");

        if (dto.EmployeeId <= 0)
            result.AddError(nameof(dto.EmployeeId), "Valid Employee ID is required");

        if (dto.ReportNumber <= 0)
            result.AddError(nameof(dto.ReportNumber), "Report Number must be greater than 0");

        if (dto.StartDate != null && dto.SubmitDate != null && dto.SubmitDate < dto.StartDate)
            result.AddError(nameof(dto.SubmitDate), "Submit date cannot be before start date");

        if (dto.SubmitDate != null && dto.DistributeDate != null && dto.DistributeDate < dto.SubmitDate)
            result.AddError(nameof(dto.DistributeDate), "Distribute date cannot be before submit date");

        return result;
    }

    public static ValidationResultDto Validate(ReportUpdateDto dto)
    {
        var result = new ValidationResultDto();

        if (dto.Id <= 0)
            result.AddError(nameof(dto.Id), "Valid Report ID is required");

        if (dto.JobId <= 0)
            result.AddError(nameof(dto.JobId), "Valid Job ID is required");

        if (dto.EmployeeId <= 0)
            result.AddError(nameof(dto.EmployeeId), "Valid Employee ID is required");

        if (dto.ReportNumber <= 0)
            result.AddError(nameof(dto.ReportNumber), "Report Number must be greater than 0");

        if (dto.StartDate != null && dto.SubmitDate != null && dto.SubmitDate < dto.StartDate)
            result.AddError(nameof(dto.SubmitDate), "Submit date cannot be before start date");

        if (dto.SubmitDate != null && dto.DistributeDate != null && dto.DistributeDate < dto.SubmitDate)
            result.AddError(nameof(dto.DistributeDate), "Distribute date cannot be before submit date");

        return result;
    }
}
