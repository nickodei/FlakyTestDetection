using Application.Features.Github;
using Application.Features.Github.Clients;
using Application.Features.Github.Entities;
using Application.Features.Github.Models;
using Application.Features.Tests.Entities;
using Application.Infrastructure;
using Application.LogParsers.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Features;

public class ScrapingService(IGithubApiService client, IGithubService githubService, IHttpClientFactory httpClientFactory, IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<Repository> FindOrCreateRepository(string owner, string name)
    {
        int i = 3;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var repository = await dbContext.Repositories.FirstOrDefaultAsync(x => x.Owner == owner && x.Name == name);
        if (repository is not null)
        {
            return repository;
        }

        var repositoryVm = await client.GetRepository(owner, name);
        repository = new Repository()
        {
            RepositoryId = repositoryVm.Id,
            Owner = repositoryVm.Owner.Login,
            Name = repositoryVm.Name,
            FullName = repositoryVm.FullName
        };
        
        dbContext.Repositories.Add(repository);
        await dbContext.SaveChangesAsync();
        
        return repository;
    }

    public async Task<Workflow> FindOrCreateWorkflow(long workflowId, Repository repository)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var workflow = await dbContext.Workflows.FirstOrDefaultAsync(x => x.RepositoryId == repository.RepositoryId && x.WorkflowId == workflowId);
        if (workflow is not null)
        {
            return workflow;
        }

        var githubWorkflows = await client.GetWorkflowsForRepository(repository.Owner, repository.Name);
        
        var githubWorkflow = githubWorkflows.Workflows.FirstOrDefault(x => x.Id == workflowId);
        if (githubWorkflow is null)
        {
            throw new Exception($"Workflow with this id [{workflowId}] does not exits on repository");
        }
        
        workflow = new Workflow()
        {
            WorkflowId = githubWorkflow.Id,
            RepositoryId = repository.RepositoryId,
            Name = githubWorkflow.Name,
            State = githubWorkflow.State,
            CreatedAt = githubWorkflow.CreatedAt
        };
        
        dbContext.Workflows.Add(workflow);
        await dbContext.SaveChangesAsync();
        
        return workflow;
    }

    public async Task<bool> WorkflowRunAlreadyExists(long workflowRunId, IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var workflowRun = await dbContext.WorkflowRuns.FindAsync(workflowRunId);
        return workflowRun is not null;
    }
    
    public static async Task<TestSuite> CreateTestSuiteFromParsingResult(long jobId, TestFile testFile, ApplicationDbContext dbContext)
    {
       

        return new TestSuite();
    }
}