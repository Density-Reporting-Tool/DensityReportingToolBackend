namespace DensityReportingToolBackend.Models;

public class PersonalInfo: ModelBase
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Invalid email address format")]
    public required string Email { get; set; }
    [System.ComponentModel.DataAnnotations.Phone(ErrorMessage = "Invalid phone number format")]
    public required string PhoneNumber { get; set; }
    public string? Company { get; set; }

    // Navigation properties to distinguish between employee and contractor
    public GeoPacificEmployee? Employee { get; set; }
}