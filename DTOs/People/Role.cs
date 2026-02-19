namespace DensityReportingToolBackend.DTOs.People;

public abstract record RoleBaseDto
{
    public string RoleTitle { get; init; } = string.Empty;
}

public record RoleReadDto : RoleBaseDto
{
    public int Id { get; init; }
}

public record RoleCreateDto : RoleBaseDto;

public record RoleUpdateDto : RoleBaseDto
{
    public int Id { get; init; }
}