﻿using System.Net;
using Application.Features;
using Application.Features.Github;
using Application.Features.Github.Clients;
using Application.Features.Github.Clients.Responses;
using Application.Features.Github.Entities;
using Application.Features.Github.Models;
using Application.Features.Tests.Entities;
using Application.Infrastructure;
using Application.LogParsers.Models;
using Application.LogParsers.Parsers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApp.Hubs;

namespace WebApp.Endpoints;

public static class ScraperEndpoints
{
    private static CancellationTokenSource? cancellationTokenSource;
    public static IEndpointRouteBuilder MapScraperEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/scraper/github/{owner}/{repository}/workflows", async (string owner, string repository, IGithubApiService client) =>
        {
            var workflows = await client.GetWorkflowsForRepository(owner, repository);
            return workflows.Workflows.Select(x => new WorkflowResponse(x.Id, x.Name)).ToList();
        }).Produces<List<WorkflowResponse>>();

        endpoints.MapGet("/scraper/github/{owner}/{repository}/workflows/{workflowId}/jobs", async (string owner, string repository, long workflowId, IGithubApiService client) =>
        {
            var workflowRuns = await client.GetWorkflowRuns(owner, repository, workflowId, new WorkflowRunsParams() { PerPage = 1, Page = 1 });
            var jobs = await client.GetJobsForWorkflowRun(owner, repository, workflowRuns.WorkflowRuns.First().Id);
            return jobs.Jobs.Select(x => x.Name).ToList();
        }).Produces<List<string>>();
        
        endpoints.MapPost("/scraper/start", async (ScrapingRequest request, IGithubApiService client, IGithubService githubService, IHttpClientFactory httpClientFactory, IHubContext<ScraperHub> hubContext, IDbContextFactory<ApplicationDbContext> dbContextFactory) =>
        {
            cancellationTokenSource = new CancellationTokenSource();
            await hubContext.Clients.All.SendAsync("ReceiveLog", "Starting Request");

            var scrapingStatus = new ScrapingStatus(request.JobNames);
            await hubContext.Clients.All.SendAsync("StatusUpdate", scrapingStatus);

            var scrapingService = new ScrapingService(client, githubService, httpClientFactory, dbContextFactory);
            
            var repository = await scrapingService.FindOrCreateRepository(request.Owner, request.Repository);
            var workflow   = await scrapingService.FindOrCreateWorkflow(request.WorkflowId, repository);
            
            const int itemsPerPage = 100;
            var totalAmount = int.MaxValue;
            
            for (var page = 1; (page * itemsPerPage) <= totalAmount; page++)
            {
                await hubContext.Clients.All.SendAsync("ReceiveLog", "Loading 100 Workflows to process", CancellationToken.None);
                var workflowRuns = await client.GetWorkflowRuns(request.Owner, request.Repository, workflow.WorkflowId, new WorkflowRunsParams()
                {
                    PerPage = itemsPerPage, 
                    Page = page
                });
                
                totalAmount = workflowRuns.TotalCount;
                for (var index = 0; index < workflowRuns.WorkflowRuns.Count; index++)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        await hubContext.Clients.All.SendAsync("ReceiveLog", "Request canceled",
                            CancellationToken.None);
                        return;
                    }

                    var workflowRun = await githubService.FindOrCreateWorkflowRun(workflowRuns.WorkflowRuns[index],
                        request.WorkflowId, dbContextFactory);
                    var githubJobs = await client.GetJobsForWorkflowRun(request.Owner, request.Repository,
                        workflowRun.WorkflowRunId);

                    await Parallel.ForEachAsync(request.JobNames, CancellationToken.None, async (jobName, token) =>
                    {
                        try
                        {
                            var githubJob = githubJobs.Jobs.FirstOrDefault(x => x.Name == jobName);
                            if (githubJob is null)
                            {
                                scrapingStatus.IncreaseJobNotFount(jobName);
                                return;
                            }

                            await using var dbContext =
                                await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
                            if (githubService.JobAlreadyExists(workflowRun.WorkflowRunId, jobName, dbContext))
                            {
                                return;
                            }

                            var job = githubService.CreateJob(workflowRun.WorkflowRunId, githubJob, dbContext);

                            var url = await githubService.DownloadLogFileUrl(request.Owner, request.Repository,
                                job.JobId, client);
                            if (url is null)
                            {
                                await hubContext.Clients.All.SendAsync("ReceiveLog", $"[{(page * itemsPerPage) + index + 1}/{totalAmount}] Got 404 From Url-Request");
                            }

                            using var httpClient = httpClientFactory.CreateClient();
                            await using var stream = await httpClient.GetStreamAsync(url, CancellationToken.None);

                            TestFile testFile;
                            try
                            {
                                testFile = new PlaywrightLogParser().ParseLogFile(new StreamReader(stream));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                scrapingStatus.IncreaseParsingError(jobName);
                                return;
                            }

                            var testSuite = new TestSuite()
                            {
                                JobId = job.JobId,
                                CountSkippedTests = testFile.CountSkippedTests,
                                CountFlakyTests = testFile.CountFlakyTests,
                                CountPassedTests = testFile.CountPassedTests,
                                CountFailedTests = testFile.CountFailedTests,
                            };

                            foreach (var testResult in testFile.TestResults)
                            {
                                testSuite.Tests.Add(new Test()
                                {
                                    Status = testResult.TestStatus,
                                    Name = testResult.Identifier.Name,
                                    Attempts = testResult.Attempts.ConvertAll(x => new TestAttempt()
                                    {
                                        Message = x.ErrorMessage
                                    })
                                });
                            }

                            dbContext.TestSuites.Add(testSuite);
                            await dbContext.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            scrapingStatus.IncreaseUnexpectedError(jobName);
                        }
                    });

                    await hubContext.Clients.All.SendAsync("ReceiveLog", $"[{(page * itemsPerPage) + index + 1}/{totalAmount}] Workflows done");
                    await hubContext.Clients.All.SendAsync("StatusUpdate", scrapingStatus);
                }
            }
        });
        
        endpoints.MapPost("/scraper/stop", () =>
        {
            cancellationTokenSource?.Cancel();
        });

        return endpoints;
    }
}