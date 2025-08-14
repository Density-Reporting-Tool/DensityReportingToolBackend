namespace DensityReportingToolBackend.Models
{
    public class PersonalInfo
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
    }

    public class GeoPacificEmployee: PersonalInfo
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }

    public class Role
    {
        public int Id { get; set; }
        public required string RoleTitle { get; set; }

        public ICollection<GeoPacificEmployee> Employees { get; set; } = [];
    }
}
