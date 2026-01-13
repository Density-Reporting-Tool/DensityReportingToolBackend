namespace DensityReportingToolBackend.DTOs.People;

public abstract record PersonalInfoBaseDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Company { get; init; }
}

public record PersonalInfoReadDto : PersonalInfoBaseDto
{
    public int Id { get; init; }
    public GeoPacificEmployeeReadDto? Employee { get; init; }
}

public record PersonalInfoCreateDto : PersonalInfoBaseDto;

public record PersonalInfoUpdateDto : PersonalInfoBaseDto
{
    public int Id { get; init; }
}