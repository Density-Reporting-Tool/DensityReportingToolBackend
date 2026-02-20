namespace DensityReportingToolBackend.DTOs.People;

public abstract record GeoPacificEmployeeBaseDto
{
    public int RoleId { get; init; }
}

public record GeoPacificEmployeeReadDto : GeoPacificEmployeeBaseDto
{
    public int Id { get; init; }
    public int PersonalInfoId { get; init; }
    public RoleReadDto Role { get; init; } = null!;
}

public record GeoPacificEmployeeCreateDto : GeoPacificEmployeeBaseDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record GeoPacificEmployeeUpdateDto : GeoPacificEmployeeBaseDto
{
    public int Id { get; init; }
    public string? Password { get; init; }
}