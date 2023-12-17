using Application.Features.Github;
using Application.Features.Github.Clients;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Endpoints;

public static class GithubEndpoints
{
    public static IEndpointRouteBuilder MapGithubEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("{owner}/{repo}/workflows", async (string owner, string repo, IGithubApiService githubApiService) =>
        {
            return await githubApiService.GetWorkflows(owner, repo);
        }).WithName("GetWorkflows").WithOpenApi();

        endpoints.MapGet("{owner}/{repo}/workflows/{workflowId}/runs", async (string owner, string repo, long workflowId, IGithubApiService githubApiService) =>
        {
            return await githubApiService.GetWorkflowRuns(owner, repo, workflowId, new WorkflowRunsParams());
        }).WithName("GetWorkflowRuns").WithOpenApi();

        endpoints.MapGet("{owner}/{repo}/runs/{workflowRunId}/logs", async (string owner, string repo, long workflowRunId, IGithubApiService githubApiService) =>
        {
            var response = await githubApiService.GetWorkflowRunLogs(owner, repo, workflowRunId);
            return response.RequestMessage?.RequestUri?.ToString();
        }).WithName("GetWorkflowRunLogs").WithOpenApi();

        endpoints.MapPost("extract-log/{jobName}/{stepName}", async (string jobName, string stepName, [FromBody]string downloadUrl) =>
        {
            using var client = new HttpClient();
            await using var stream = client.GetStreamAsync(downloadUrl).Result;
            
            var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);
            var folder = archive.Entries.First(entry => entry.FullName.Equals(jobName));
        }).WithName("ExtractLog").WithOpenApi();

        return endpoints;
    }
}