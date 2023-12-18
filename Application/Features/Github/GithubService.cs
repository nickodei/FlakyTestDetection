﻿using System.Net;
using Application.Features.Github.Clients;
using Application.Features.Github.Entities;
using Application.Features.Github.Models;
using Application.Features.Tests.Entities;
using Application.Infrastructure;
using Application.LogParsers;
using Application.LogParsers.Parsers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Github;

public class GithubService(IGithubApiService client, IDbContextFactory<ApplicationDbContext> dbContextFactory) : IGithubService
{
    public async Task StartGithubScraping(ScrapingRequest request, ILogFileParser parser, Func<string, Task> logging, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        await logging($"Starting Log-Scraping on Github for: {request.Owner}/{request.Repository}");
        await logging("--------------------------------------------------------------------------");
        
        try
        {
            var repository = await FindOrCreateRepository(request, dbContext, logging);
            var workflow   = await FindOrCreateWorkflow(repository, request.WorkflowName, dbContext, logging);
            if (workflow is null)
            {
                throw new Exception($"Couldn't find Workflow [{request.WorkflowName}] for Repository {request.Owner}/{request.Repository}");
            }

            await logging($"Fetching Workflow-runs for Workflow [{request.WorkflowName}]:");
            
            var totalCount = int.MaxValue;
            for (var page = 1; totalCount == int.MaxValue || page < (totalCount / 30); page++)
            {
                var workflowRuns = await client.GetWorkflowRuns(request.Owner, request.Repository, workflow.WorkflowId, new WorkflowRunsParams() { PerPage = 100, Page = page });
                if (totalCount == int.MaxValue)
                {
                    totalCount = workflowRuns.TotalCount;
                    
                    await logging($"Found {workflowRuns.TotalCount} Workflow-runs. Trying to parse their logs:");
                    await logging("--------------------------------------------------------------------------");
                }
                
                for (var index = 0; index < workflowRuns.WorkflowRuns.Count; index++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        await dbContext.DisposeAsync();
                        await logging($"Scraper got canceled!");
                        
                        cancellationToken.ThrowIfCancellationRequested();   
                    }
                    
                    var prefix = $"[{(page - 1) * 100 + index + 1}/{totalCount}]";
                    await logging($"{prefix}: Trying to process workflow-run with given job-name");
                    
                    var workflowRun =  await FindOrCreateWorkflowRun(workflow, workflowRuns.WorkflowRuns[index], dbContext, prefix);
                    var githubJob = await dbContext.Jobs.FirstOrDefaultAsync(x => x.WorkflowRunId == workflowRun.WorkflowRunId && x.Name == request.Job);
                    if (githubJob is not null)
                    {
                        await logging($"{prefix}: Already tried to parse Job [{request.Job}] for this workflow-run. Skipping");
                        continue;
                    }

                    githubJob = await CreateGithubJob(repository, workflowRun, request.Job, dbContext, logging, prefix);
                    if (githubJob is null)
                    {
                        continue;
                    }

                    var downloadUrl = await GetDownloadUrlForJobLog(repository, githubJob, dbContext, logging, prefix);
                    if (downloadUrl is null)
                    {
                        await logging($"{prefix}: No logs to be found. Skipping this workflow-run");
                        continue;
                    }
                    
                    await logging($"{prefix}: Download-url [{downloadUrl}]");
                    
                    using var httpClient = new HttpClient();
                    await using var stream = await httpClient.GetStreamAsync(downloadUrl);

                    try
                    {
                        var testSuite = await ParseTestsFromLogfile(stream, logging, prefix);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        await logging($"{prefix}: Error while parsing the file [{e.Message}]. Skipping.");
                        continue;
                    }
                    
                    await logging("--------------------------------------------------------------------------");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await logging($"An error occured. Aborting.");
        }
    }
    
    private async Task<Repository> FindOrCreateRepository(ScrapingRequest request, ApplicationDbContext dbContext, Func<string, Task> log)
    {
        await log($"Trying to get Repository {request.Owner}/{request.Repository}:");
        var repo = await dbContext.Repositories.FirstOrDefaultAsync(x => x.Owner == request.Owner && x.Name == request.Repository);
        if (repo is not null)
        {
            await log($"Found Repository {request.Owner}/{request.Repository}");
            return repo;
        }

        var githubRepo = await client.GetRepository(request.Owner, request.Repository);
        repo = new Repository()
        {
            RepositoryId = githubRepo.Id,
            Owner = githubRepo.Owner.Login,
            Name = githubRepo.Name,
            FullName = githubRepo.FullName
        };
        
        dbContext.Repositories.Add(repo);
        await dbContext.SaveChangesAsync();

        await log($"Created Repository {request.Owner}/{request.Repository}");
        return repo;
    }

    private async Task<Workflow?> FindOrCreateWorkflow(Repository repository, string name, ApplicationDbContext dbContext, Func<string, Task> log)
    {
        await log($"Trying to get Workflow {name}:");
        var workflow = await dbContext.Workflows.FirstOrDefaultAsync(x => x.RepositoryId == repository.RepositoryId && name == x.Name);
        if (workflow is not null)
        {
            await log($"Found Workflow {name}");
            return workflow;
        }

        var githubWorkflows = await client.GetWorkflowsForRepository(repository.Owner, repository.Name);
        var githubWorkflow = githubWorkflows.Workflows.FirstOrDefault(x => x.Name == name);
        if (githubWorkflow is null)
        {
            return null;
        }
        
        workflow = new Workflow()
        {
            RepositoryId = repository.RepositoryId,
            WorkflowId = githubWorkflow.Id,
            Name = githubWorkflow.Name,
        };
        
        dbContext.Workflows.Add(workflow);
        await dbContext.SaveChangesAsync();

        await log($"Created Workflow (yml) {name}");
        return workflow;
    }
    
    private async Task<WorkflowRun> FindOrCreateWorkflowRun(Workflow workflow, WorkflowRunVm workflowRunVm, ApplicationDbContext dbContext, string prefix)
    {
        var workflowRun = await dbContext.WorkflowRuns.FirstOrDefaultAsync(x => x.WorkflowRunId == workflowRunVm.Id);
        if (workflowRun is not null)
        {
            return workflowRun;
        }
        
        workflowRun = new WorkflowRun()
        {
            WorkflowId = workflow.WorkflowId,
            WorkflowRunId = workflowRunVm.Id,
            Name = workflowRunVm.DisplayTitle,
            Status = workflowRunVm.Status,
            CreatedAt = workflowRunVm.CreatedAt
        };
            
        dbContext.WorkflowRuns.Add(workflowRun);
        await dbContext.SaveChangesAsync();

        return workflowRun;
    }

    private async Task<Job?> CreateGithubJob(Repository repository, WorkflowRun workflowRun, string jobName, ApplicationDbContext dbContext, Func<string, Task> log, string prefix)
    {
        var githubJobs = await client.GetJobsForWorkflowRun(repository.Owner, repository.Name, workflowRun.WorkflowRunId);
        
        var githubJob = githubJobs.Jobs.FirstOrDefault(x => x.Name == jobName);
        if (githubJob is null)
        {
            await log($"{prefix}: Couldn't find Job with Name [{jobName}]");
            return null;
        }
        
        var job = new Job()
        {
            JobId = githubJob.Id,
            WorkflowRunId = workflowRun.WorkflowRunId,
            Name = githubJob.Name
        };
            
        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync();

        return job;
    }

    private async Task<Uri?> GetDownloadUrlForJobLog(Repository repository, Job job, ApplicationDbContext dbContext, Func<string, Task> log, string prefix)
    {
        await log($"{prefix}: Getting download-url for logs:");
        var httpResponse = await client.GetDownloadUrlForJobLogs(repository.Owner, repository.Name, job.JobId);
        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
                
        var logUrl = httpResponse.RequestMessage?.RequestUri;
        if (logUrl is not null)
        {
            await log($"{prefix}: Successfully got log url.");
        }

        return logUrl;
    }

    private async Task<TestSuite> ParseTestsFromLogfile(Stream fileStream, Func<string, Task> log, string prefix)
    {
        var testFile = new PlaywrightLogParser().ParseLogFile(new StreamReader(fileStream));
        await log($"{prefix}: Found {testFile.CountPassedTests} passed, {testFile.CountFlakyTests} flaky and {testFile.CountFailedTests} failed tests in log");
        return new TestSuite();
    }
}