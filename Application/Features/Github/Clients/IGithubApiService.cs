using Application.Features.Github.Clients.Responses;
using Refit;

namespace Application.Features.Github.Clients;

public class WorkflowRunsParams
{
    [AliasAs("per_page")]
    public int PerPage { get; set; } = 100;
    
    [AliasAs("page")]
    public int Page { get; set; } = 1;
}

public interface IGithubApiService
{
    [Get("/repos/{owner}/{repo}")]
    Task<RepositoryResult> GetRepository(string owner, string repo);
    
    [Get("/repos/{owner}/{repo}/actions/workflows")]
    Task<WorkflowsResult> GetWorkflowsForRepository(string owner, string repo);
    
    [Get("/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs")]
    Task<WorkflowRunsResult> GetWorkflowRuns(string owner, string repo, long workflowId, WorkflowRunsParams queryParams);
    
    [Get("/repos/{owner}/{repo}/actions/runs/{runId}/jobs")]
    Task<JobsResult> GetJobsForWorkflowRun(string owner, string repo, long runId);
    
    [Get("/repos/{owner}/{repo}/actions/jobs/{jobId}/logs")]
    Task<HttpResponseMessage> GetDownloadUrlForJobLogs(string owner, string repo, long jobId);

}