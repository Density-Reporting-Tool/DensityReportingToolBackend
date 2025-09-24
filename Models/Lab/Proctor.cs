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