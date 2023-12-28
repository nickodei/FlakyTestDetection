namespace Application.Features.Github.Models;

public enum SkippingRequest
{
    Job,
    Workflow
}

public class ScrapingRequest
{
    public string Owner { get; set; }
    public string Repository { get; set; }
    public SkippingRequest SkippingRequest { get; set; }
    
    public long WorkflowId { get; set; }
    public List<string> JobNames { get; set; } = [];
}