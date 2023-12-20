using Application.Features.Github.Models;
using Refit;
namespace WebApp.Clients;

public interface IScraperClient
{
    [Get("/scraper/github/{owner}/{repository}/workflows")]
    Task<List<WorkflowResponse>> GetWorkflows(string owner, string repository);

    [Get("/scraper/github/{owner}/{repository}/workflows/{workflowId}/jobs")]
    Task<List<string>> GetJobs(string owner, string repository, long workflowId);
    
    [Post("/scraper/start")]
    Task StartScraping(ScrapingRequest request);
    
    [Post("/scraper/stop")]
    Task StopScraping();
}