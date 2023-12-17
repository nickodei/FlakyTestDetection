using Application.Features.Github.Models;
using Application.Features.Github.Responses;
using Refit;

namespace Application.Features.Github.Clients;

public class WorkflowRunsParams
{
    [AliasAs("per_page")]
    public int PerPage { get; set; } = 100;
    public int Page { get; set; } = 1;
}

public interface IGithubApiService
{
    [Get("/repos/{owner}/{repo}/actions/workflows")]
    Task<GetWorkflowsResponse> GetWorkflows(string owner, string repo);
    
    [Get("/repos/{owner}/{repo}/actions/workflows/{name}")]
    Task<Workflow> GetWorkflow(string owner, string repo, string name);

    [Get("/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs")]
    Task<GetWorkflowRunsResponse> GetWorkflowRuns(string owner, string repo, long workflowId, WorkflowRunsParams queryParams);
    
    [Get("/repos/{owner}/{repo}/actions/runs/{workflowRunId}/logs")]
    Task<HttpResponseMessage> GetWorkflowRunLogs(string owner, string repo, long workflowRunId);
}