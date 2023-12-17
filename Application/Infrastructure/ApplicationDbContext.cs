using Application.Features.Github.Entities;
using Application.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Repository> Repositories { get; set; }
    public DbSet<WorkflowRun> WorkflowRuns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkflowRunEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}