namespace Application.Features.Github.Models;

public class ScrapingRequest
{
    public string Owner { get; set; }
    public string Repository { get; set; }
    public string WorkflowName { get; set; }
    public string Job { get; set; }
    public string LogFilePath { get; set; }
}