namespace DensityReportingToolBackend.Models;

public class Proctor
{
    public int Id { get; set; }
    
    public string? ProctorID { get; set; }
    public string? ProctorTestNumber { get; set; }   
    
    public int LabTestId { get; set; }
    public LabTest LabTest { get; set; } = null!;
    
    public int? SieveId { get; set; }
    public Sieve? Sieve { get; set; }
    
    public int ProctorTypeId { get; set; }
    public ProctorType ProctorType { get; set; } = null!;
    
    // Test details
    public string? MaterialType { get; set; }         
    public string? LabLocation { get; set; }        
    public DateTime? DateSampled { get; set; }       
    public DateTime? DateTested { get; set; }        
    
    // Test results
    public double? MaxDensity { get; set; }
    public double? CorrectedDensity { get; set; }
    public double? OptimumMoistureContent { get; set; }
    public double? SpecificGravity { get; set; }
    public double? OversizePercentage { get; set; }   
    
    public ICollection<ProctorAdditionalJob> AdditionalJobs { get; set; } = [];
    public ICollection<DensityTest> DensityTests { get; set; } = [];
}

/// <summary>
/// Extension methods for Proctor entity
/// </summary>
public static class ProctorExtensions
{
    /// <summary>
    /// Converts Proctor entity to ProctorReadDto
    /// </summary>
    public static ProctorReadDTOLD ToDto(this Proctor proctor, HashSet<(Type, int)>? visited = null)
    {
        visited ??= new HashSet<(Type, int)>();
        return new ProctorReadDTOLD(proctor, visited);
    }
}

/// <summary>
/// Base DTO containing common proctor properties
/// </summary>
public class ProctorBaseDto
{
    public string JobNumber { get; set; } = string.Empty;
    public string? ProctorId { get; set; }
    public string? ProctorTestNumber { get; set; }
    public string? MaterialType { get; set; }
    public string? LabLocation { get; set; }
    public string? ProctorType { get; set; }
    public DateTime? DateSampled { get; set; }
    public DateTime? DateTested { get; set; }
    public double? MaxDensity { get; set; }
    public double? CorrectedDensity { get; set; }
    public double? OptimumMoisture { get; set; }
    public double? SpecificGravity { get; set; }
    public double? OversizePercentage { get; set; }
}

/// <summary>
/// DTO for creating a new proctor
/// </summary>
public class ProctorCreateDto : ProctorBaseDto
{
}

/// <summary>
/// DTO for updating an existing proctor
/// </summary>
public class ProctorUpdateDto : ProctorBaseDto
{
    public int Id { get; set; }
}

/// <summary>
/// DTO for reading proctor data with related entities
/// </summary>
public class ProctorReadDTOLD : ProctorBaseDto // TODO: Remove this in favor of unified server framework
{
    public int Id { get; set; }
    public int LabTestId { get; set; }
    public int ProctorTypeId { get; set; }
    
    // Related entities
    public string JobId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    
    public ProctorReadDTOLD(Proctor proctor, HashSet<(Type, int)> visited)
    {
        Id = proctor.Id;
        ProctorId = proctor.ProctorID;
        ProctorTestNumber = proctor.ProctorTestNumber;
        LabTestId = proctor.LabTestId;
        ProctorTypeId = proctor.ProctorTypeId;
        
        // Test details
        MaterialType = proctor.MaterialType;
        LabLocation = proctor.LabLocation;
        DateSampled = proctor.DateSampled;
        DateTested = proctor.DateTested;
        
        // Test results
        MaxDensity = proctor.MaxDensity;
        CorrectedDensity = proctor.CorrectedDensity;
        OptimumMoisture = proctor.OptimumMoistureContent;
        SpecificGravity = proctor.SpecificGravity;
        OversizePercentage = proctor.OversizePercentage;
        
        // Related data from navigation properties
        ProctorType = proctor.ProctorType?.Type;
        JobNumber = proctor.LabTest?.Job?.JobNumber ?? string.Empty;
        JobId = proctor.LabTest?.Job?.Id.ToString() ?? string.Empty;
        ClientName = proctor.LabTest?.Job?.ClientName ?? string.Empty;
        ProjectName = proctor.LabTest?.Job?.ProjectName ?? string.Empty;
    }
}