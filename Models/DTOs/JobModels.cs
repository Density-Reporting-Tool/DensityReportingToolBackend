namespace DensityReportingToolBackend.Models
{
    /// <summary>
    /// DTO for assigning a project manager to a job
    /// </summary>
    public class AssignProjectManagerDto
    {
        public int PersonalInfoId { get; set; }
        public bool IsPrimary { get; set; } = true;
    }

    /// <summary>
    /// DTO for assigning a site contact to a job
    /// </summary>
    public class AssignSiteContactDto
    {
        public int PersonalInfoId { get; set; }
        public bool IsPrimary { get; set; } = true;
    }
}

