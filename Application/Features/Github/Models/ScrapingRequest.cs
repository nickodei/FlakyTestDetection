namespace Application.Features.Github.Models;

public class ScrapingRequest
{
    public string Owner { get; set; }
    public string Repository { get; set; }
    
    public long WorkflowId { get; set; }
    public List<string> JobNames { get; set; } = [];
}