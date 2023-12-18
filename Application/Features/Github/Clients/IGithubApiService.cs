using Application.Features.Github.Clients.Responses;
using Application.Features.Github.Entities;
using Application.Features.Github.Models;
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
    [Get("/repos/{owner}/{repo}")]
    Task<GetRepositoryResponse> GetRepository(string owner, string repo);
    
    [Get("/repos/{owner}/{repo}/actions/workflows")]
    Task<WorkflowListResult> GetWorkflowsForRepository(string owner, string repo);
    
    [Get("/repos/{owner}/{repo}/actions/workflows/{name}")]
    Task<WorkflowResult> GetWorkflow(string owner, string repo, string name);
    
    [Get("/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs")]
    Task<WorkflowRunResult> GetWorkflowRuns(string owner, string repo, long workflowId, WorkflowRunsParams queryParams);
    
    [Get("/repos/{owner}/{repo}/actions/runs/{runId}/jobs")]
    Task<JobsResult> GetJobsForWorkflowRun(string owner, string repo, long runId);
    
    [Get("/repos/{owner}/{repo}/actions/jobs/{jobId}/logs")]
    Task<HttpResponseMessage> GetDownloadUrlForJobLogs(string owner, string repo, long jobId);
    
    
    
    
    
    
    
    [Get("/repos/{owner}/{repo}/actions/workflows")]
    Task<GetWorkflowsResponse> GetWorkflows(string owner, string repo);
    



    
    [Get("/repos/{owner}/{repo}/actions/runs/{workflowRunId}/logs")]
    Task<HttpResponseMessage> GetWorkflowRunLogs(string owner, string repo, long workflowRunId);
}