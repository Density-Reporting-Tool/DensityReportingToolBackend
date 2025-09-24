namespace DensityReportingToolBackend.Models;

public class PersonalInfo: ModelBaseWithDto<PersonalInfo, PersonalInfoReadDto>
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

public class PersonalInfoBaseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Company { get; set; }
}

public class PersonalInfoCreateDto : PersonalInfoBaseDto { }

public class PersonalInfoUpdateDto : PersonalInfoBaseDto { }

public class PersonalInfoReadDto : PersonalInfoBaseDto
{
    public int Id { get; set; }

    public PersonalInfoReadDto(PersonalInfo info, HashSet<(Type, int)> visited)
    {
        Id = info.Id;
        FirstName = info.FirstName;
        LastName = info.LastName;
        Email = info.Email;
        PhoneNumber = info.PhoneNumber;
        Company = info.Company;
    }
}