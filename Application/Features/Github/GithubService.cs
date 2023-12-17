using System.Net;
using Application.Features.Github.Clients;
using Application.Features.Github.Entities;
using Application.Features.Github.Models;
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
        
        try
        {
            await logging($"Starting Log-Scraping on Github for: {request.Owner}/{request.Repository}");
            await logging("--------------------------------------------------------------------------");

            await logging($"Fetching Workflow with Name: {request.WorkflowName}");
            var workflow = await client.GetWorkflow(request.Owner, request.Repository, request.WorkflowName);
            await logging($"Found Workflow [{request.WorkflowName}] with id: {workflow.Id}");

            var repository = await FindOrCreateRepository(request, dbContext);
            
            await logging($"Fetching Workflow-runs for Workflow [{request.WorkflowName}]:");
            var workflowRuns = await client.GetWorkflowRuns(request.Owner, request.Repository, workflow.Id, new WorkflowRunsParams() { PerPage = 100 });
            await logging($"Found {workflowRuns.TotalCount} Workflow-runs. Trying to parse their logs:");
            await logging("--------------------------------------------------------------------------");
            
            for (var page = 1; page < (workflowRuns.TotalCount / 30);)
            {
                for (var index = 0; index < workflowRuns.WorkflowRuns.Count; index++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        await dbContext.DisposeAsync();
                        cancellationToken.ThrowIfCancellationRequested();   
                    }
                    
                    var workflowRunVm = workflowRuns.WorkflowRuns[index];
                    await FindOrCreateWorkflowRun(repository, workflowRunVm, dbContext);
                
                    await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} Trying to download Logs for Workflow-run [{workflowRunVm.DisplayTitle}]:");
                    var httpResponse = await client.GetWorkflowRunLogs(request.Owner, request.Repository, workflowRunVm.Id);
                    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} No logs to be found. Skipping this workflow");
                        continue;
                    }
                
                    var logUrl = httpResponse.RequestMessage?.RequestUri;
                    if (logUrl is not null)
                    {
                        await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} Successfully got log url.");
                    }
                
                    using var httpClient = new HttpClient();
                    await using var stream = httpClient.GetStreamAsync(logUrl).Result;
                    await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} Successfully downloaded logs.");
                
                    var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);
                    await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} Log has {archive.Entries.Count} Entries.");
                    
                    var entry = archive.GetEntry(request.LogFilePath);
                    if (entry is null)
                    {
                        await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} File could not be found under the path: {request.LogFilePath}. Skipping.");
                        await logging("--------------------------------------------------------------------------");
                        continue;
                    }

                    using var streamReader = new StreamReader(entry.Open());
                    try
                    {
                        var testFile = new PlaywrightLogParser().ParseLogFile(streamReader);
                        await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} Found [passed: {testFile.CountPassedTests}, failed: {testFile.CountFailedTests}, flaky: {testFile.CountFlakyTests}] in TestFile.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        await logging($"{WorkflowPrefix(page, index, workflowRuns.TotalCount)} Error while parsing the file: {e.Message}. Skipping.");
                        await logging("--------------------------------------------------------------------------");
                        continue;
                    }
                    
                    await logging("--------------------------------------------------------------------------");
                }

                page++;
                workflowRuns = await client.GetWorkflowRuns(request.Owner, request.Repository, workflow.Id, new WorkflowRunsParams() { PerPage = 100, Page = page});
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await logging($"An error occured. Aborting.");
        }
    }

    private async Task<Repository> FindOrCreateRepository(ScrapingRequest request, ApplicationDbContext dbContext)
    {
        var repository = await dbContext.Repositories.SingleOrDefaultAsync(x =>
            x.Owner == request.Owner && x.Name == request.Repository
        );
        if (repository is not null)
        {
            return repository;
        }
        
        repository = new Repository()
        {
            Owner = request.Owner,
            Name = request.Repository
        };
            
        dbContext.Repositories.Add(repository);
        await dbContext.SaveChangesAsync();

        return repository;
    }
    
    private async Task<WorkflowRun> FindOrCreateWorkflowRun(Repository repository, WorkflowRunVm workflowRunVm, ApplicationDbContext dbContext)
    {
        var workflowRun = await dbContext.WorkflowRuns.SingleOrDefaultAsync(x =>
            x.WorkflowRunId == workflowRunVm.Id
        );
        if (workflowRun is not null)
        {
            return workflowRun;
        }
        
        workflowRun = new WorkflowRun()
        {
            WorkflowName = workflowRunVm.DisplayTitle,
            WorkflowRunId = workflowRunVm.Id,
            RepositoryId = repository.Id,
            Repository = repository,
        };
            
        dbContext.WorkflowRuns.Add(workflowRun);
        await dbContext.SaveChangesAsync();

        return workflowRun;
    }
    private static string WorkflowPrefix(int page, int index, int totalCount) => $"[{(page - 1) * 100 + index + 1}/{totalCount}]:";
}