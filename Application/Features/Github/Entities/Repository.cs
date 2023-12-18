namespace Application.Features.Github.Entities;

public class Repository
{
    public long RepositoryId { get; set; }
    public required string Owner { get; set; }
    public required string Name { get; set; }
    public required string FullName { get; set; }

    public ICollection<Workflow> Workflows { get; set; } = [];
}