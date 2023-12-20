using Application.Features.Github.Entities;
using Application.Features.Tests.Entities;
using Application.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Repository> Repositories { get; set; }
    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<WorkflowRun> WorkflowRuns { get; set; }

    public DbSet<TestSuite> TestSuites { get; set; } 
    public DbSet<Test> Tests { get; set; } 
    public DbSet<TestAttempt> TestAttempts { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkflowRunEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}