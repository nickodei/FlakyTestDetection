namespace Application.Features.Github.Clients.Responses;

public class WorkflowResult
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string State { get; set; }
    public required DateTime CreatedAt { get; set; }
}