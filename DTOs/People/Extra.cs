namespace DensityReportingToolBackend.DTOs.People;

// Flat representation of an Employee (formerly the anonymous object in GetEmployee)
public record GeoPacificEmployeeFlatDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public int RoleId { get; init; }
    public string? RoleTitle { get; init; }
    public string PersonType { get; init; } = "GeoPacific Employee";
}

public record PersonListFlatDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Company { get; init; }
    public string? PersonType { get; set; }
    public string? Role { get; set; }
}