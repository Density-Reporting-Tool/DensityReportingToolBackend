namespace DensityReportingToolBackend.Validators;

public class ValidationResultDto
{
    public Dictionary<string, List<string>> Errors { get; set; } = [];

    public bool IsValid => Errors.Count == 0;

    public void AddError(string field, string message)
    {
        if (!Errors.ContainsKey(field))
            Errors[field] = new List<string>();
        Errors[field].Add(message);
    }
}