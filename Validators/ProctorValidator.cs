using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;

namespace DensityReportingToolBackend.Validators;

public static class ProctorValidator
{
    public static ValidationResultDto Validate(ProctorCreateDto dto)
    {
        var result = new ValidationResultDto();

        if (string.IsNullOrWhiteSpace(dto.JobNumber))
            result.AddError(nameof(dto.JobNumber), "Job Number is required");

        if (string.IsNullOrWhiteSpace(dto.ProctorId))
            result.AddError(nameof(dto.ProctorId), "Proctor ID is required");

        if (string.IsNullOrWhiteSpace(dto.MaterialType))
            result.AddError(nameof(dto.MaterialType), "Material Type is required");

        if (string.IsNullOrWhiteSpace(dto.LabLocation))
            result.AddError(nameof(dto.LabLocation), "Lab Location is required");

        if (string.IsNullOrWhiteSpace(dto.ProctorType))
            result.AddError(nameof(dto.ProctorType), "Proctor Type is required");
        else if (dto.ProctorType != "Modified" && dto.ProctorType != "Standard")
            result.AddError(nameof(dto.ProctorType), "Proctor Type must be Modified or Standard");

        if (dto.MaxDensity.HasValue && (dto.MaxDensity <= 0 || dto.MaxDensity > 3000))
            result.AddError(nameof(dto.MaxDensity), "Max Density must be between 0 and 3000 kg/m³");

        if (dto.CorrectedDensity.HasValue && (dto.CorrectedDensity <= 0 || dto.CorrectedDensity > 3000))
            result.AddError(nameof(dto.CorrectedDensity), "Corrected Density must be between 0 and 3000 kg/m³");

        if (dto.OptimumMoisture < 0 || dto.OptimumMoisture > 100)
            result.AddError(nameof(dto.OptimumMoisture), "Optimum Moisture must be between 0 and 100");

        if (dto.OversizePercentage < 0 || dto.OversizePercentage > 100)
            result.AddError(nameof(dto.OversizePercentage), "Oversize Percentage must be between 0 and 100");

        if (dto.SpecificGravity.HasValue && (dto.SpecificGravity <= 0 || dto.SpecificGravity > 10))
            result.AddError(nameof(dto.SpecificGravity), "Specific Gravity must be between 0 and 10");

        if (dto.DateSampled.HasValue && dto.DateTested.HasValue && dto.DateTested < dto.DateSampled)
            result.AddError(nameof(dto.DateTested), "Date Tested cannot be before Date Sampled");

        return result;
    }

    public static ValidationResultDto Validate(ProctorUpdateDto dto)
    {
        var result = new ValidationResultDto();

        if (dto.Id <= 0)
            result.AddError(nameof(dto.Id), "Valid Proctor ID is required");

        if (!string.IsNullOrWhiteSpace(dto.ProctorType) && dto.ProctorType != "Modified" && dto.ProctorType != "Standard")
            result.AddError(nameof(dto.ProctorType), "Proctor Type must be Modified or Standard");

        if (dto.MaxDensity.HasValue && (dto.MaxDensity <= 0 || dto.MaxDensity > 3000))
            result.AddError(nameof(dto.MaxDensity), "Max Density must be between 0 and 3000 kg/m³");

        if (dto.CorrectedDensity.HasValue && (dto.CorrectedDensity <= 0 || dto.CorrectedDensity > 3000))
            result.AddError(nameof(dto.CorrectedDensity), "Corrected Density must be between 0 and 3000 kg/m³");

        if (dto.OptimumMoisture.HasValue && (dto.OptimumMoisture < 0 || dto.OptimumMoisture > 100))
            result.AddError(nameof(dto.OptimumMoisture), "Optimum Moisture must be between 0 and 100");

        if (dto.OversizePercentage.HasValue && (dto.OversizePercentage < 0 || dto.OversizePercentage > 100))
            result.AddError(nameof(dto.OversizePercentage), "Oversize Percentage must be between 0 and 100");

        if (dto.SpecificGravity.HasValue && (dto.SpecificGravity <= 0 || dto.SpecificGravity > 10))
            result.AddError(nameof(dto.SpecificGravity), "Specific Gravity must be between 0 and 10");

        if (dto.DateSampled.HasValue && dto.DateTested.HasValue && dto.DateTested < dto.DateSampled)
            result.AddError(nameof(dto.DateTested), "Date Tested cannot be before Date Sampled");

        return result;
    }
}
