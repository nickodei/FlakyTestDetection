using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using WebAPI.Utility;

namespace WebAPI.Github;

public class GithubModule : IModule
{
    // owner: WordPress
    // repo: gutenberg
    // WorkflowId: 7131644069
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRefitClient<IGithubApiService>(new RefitSettings 
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            })
        })
        .ConfigureHttpClient(client =>
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["GithubToken"]);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("flaky-test-detection");
        });
        
        return services;
    }

    IEndpointRouteBuilder IModule.MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("{owner}/{repo}/workflows", async (string owner, string repo, IGithubApiService githubApiService) =>
        {
            return await githubApiService.GetWorkflows(owner, repo);
        }).WithName("GetWorkflows").WithOpenApi();

        endpoints.MapGet("{owner}/{repo}/workflows/{workflowId}/runs", async (string owner, string repo, long workflowId, IGithubApiService githubApiService) =>
        {
            return await githubApiService.GetWorkflowRuns(owner, repo, workflowId);
        }).WithName("GetWorkflowRuns").WithOpenApi();

        endpoints.MapGet("{owner}/{repo}/runs/{workflowRunId}/logs", async (string owner, string repo, long workflowRunId, IGithubApiService githubApiService) =>
        {
            var response = await githubApiService.GetWorkflowRunLogs(owner, repo, workflowRunId);
            return response.RequestMessage?.RequestUri?.ToString();
        }).WithName("GetWorkflowRunLogs").WithOpenApi();

        endpoints.MapPost("extract-log/{jobName}/{stepName}", async (string jobName, string stepName, [FromBody]string downloadUrl) =>
        {
            using (var client = new HttpClient())
            using (var stream = client.GetStreamAsync(downloadUrl).Result)
            {
                var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);
                var folder = archive.Entries.First(entry => entry.FullName.Equals(jobName));
                //folder.Open()
            }
        }).WithName("ExtractLog").WithOpenApi();

        return endpoints;
    }
}