namespace DensityReportingToolBackend.Models;

public abstract class ModelBase
{
    public int Id { get; set; }

    protected bool AlreadyVisited(HashSet<(Type, int)> visited)
    {
        var key = (GetType(), Id);
        if (visited.Contains(key))
            return true;

        visited.Add(key);
        return false;
    }
}

public abstract class ModelBaseWithDto<TModel, TDto> : ModelBase
    where TDto : class
{
    public TDto? ToDto()
    {
        return this.ToDto([]);
    }

    public TDto? ToDto(HashSet<(Type, int)> visited)
    {
        if (this.AlreadyVisited(visited))
            return null;

        visited.Add((typeof(TModel), Id));

        return (TDto?)Activator.CreateInstance(typeof(TDto), this, visited);
    }
}