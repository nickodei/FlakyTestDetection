using Application.Features.Github.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.EntityConfigurations;

public class WorkflowRunEntityTypeConfiguration : IEntityTypeConfiguration<WorkflowRun>
{
    public void Configure(EntityTypeBuilder<WorkflowRun> builder)
    {
        //builder.HasOne(e => e.GithubRepository)
        //    .WithMany(e => e.WorkflowRuns)
        //    .HasForeignKey(e => e.RepositoryId)
        //    .IsRequired();
    }
}