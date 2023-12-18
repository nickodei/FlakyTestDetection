namespace Application.Features.Github.Clients.Responses;

public class GetRepositoryResponse
{
    public required int Id { get; set; }
    public required OwnerResponse Owner { get; set; }
    public required string Name { get; set; }
    public required string FullName { get; set; }
}

public class OwnerResponse
{
    public required string Login { get; set; }
}