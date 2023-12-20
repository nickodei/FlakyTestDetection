using Application.Features.Github.Clients;
using Application.Features.Github.Clients.Responses;
using Application.Features.Github.Entities;
using Application.Features.Github.Models;
using Application.Infrastructure;
using Application.LogParsers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Github;

public interface IGithubService
{
    Task<Uri?> DownloadLogFileUrl(string owner, string repository, long jobId, IGithubApiService client);
    Job CreateJob(long workflowRunId, JobResult jobResult, ApplicationDbContext dbContext);
    bool JobAlreadyExists(long workflowRunId, string jobName, ApplicationDbContext dbContext);
    Task<WorkflowRun> FindOrCreateWorkflowRun(WorkflowRunResult githubWorkflowRun, long workflowId, IDbContextFactory<ApplicationDbContext> dbContextFactory);
    Task StartGithubScraping(ScrapingRequest request, ILogFileParser parser, Func<string, Task> logging, CancellationToken cancellationToken);
}