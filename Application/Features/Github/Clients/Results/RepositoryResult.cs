namespace Application.Features.Github.Clients.Responses;

public class RepositoryResult
{
    public required int Id { get; set; }
    public required OwnerResult Owner { get; set; }
    public required string Name { get; set; }
    public required string FullName { get; set; }
}

public class OwnerResult
{
    public required string Login { get; set; }
}