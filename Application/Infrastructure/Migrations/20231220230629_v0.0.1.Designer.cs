﻿// <auto-generated />
using System;
using Application.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Application.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231220230629_v0.0.1")]
    partial class v001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Application.Features.Github.Entities.Job", b =>
                {
                    b.Property<long>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("JobId"));

                    b.Property<string>("Conclusion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("WorkflowRunId")
                        .HasColumnType("bigint");

                    b.HasKey("JobId");

                    b.HasIndex("WorkflowRunId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.Repository", b =>
                {
                    b.Property<long>("RepositoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("RepositoryId"));

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RepositoryId");

                    b.ToTable("Repositories");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.Workflow", b =>
                {
                    b.Property<long>("WorkflowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("WorkflowId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("RepositoryId")
                        .HasColumnType("bigint");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("WorkflowId");

                    b.HasIndex("RepositoryId");

                    b.ToTable("Workflows");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.WorkflowRun", b =>
                {
                    b.Property<long>("WorkflowRunId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("WorkflowRunId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("WorkflowId")
                        .HasColumnType("bigint");

                    b.HasKey("WorkflowRunId");

                    b.HasIndex("WorkflowId");

                    b.ToTable("WorkflowRuns");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("TestSuiteId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TestSuiteId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.TestAttempt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<int>("TestId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("TestAttempts");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.TestSuite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CountFailedTests")
                        .HasColumnType("integer");

                    b.Property<int>("CountFlakyTests")
                        .HasColumnType("integer");

                    b.Property<int>("CountPassedTests")
                        .HasColumnType("integer");

                    b.Property<int>("CountSkippedTests")
                        .HasColumnType("integer");

                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.ToTable("TestSuites");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.Job", b =>
                {
                    b.HasOne("Application.Features.Github.Entities.WorkflowRun", "WorkflowRun")
                        .WithMany("Jobs")
                        .HasForeignKey("WorkflowRunId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WorkflowRun");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.Workflow", b =>
                {
                    b.HasOne("Application.Features.Github.Entities.Repository", "Repository")
                        .WithMany("Workflows")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.WorkflowRun", b =>
                {
                    b.HasOne("Application.Features.Github.Entities.Workflow", "Workflow")
                        .WithMany("WorkflowRuns")
                        .HasForeignKey("WorkflowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workflow");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.Test", b =>
                {
                    b.HasOne("Application.Features.Tests.Entities.TestSuite", "TestSuite")
                        .WithMany("Tests")
                        .HasForeignKey("TestSuiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TestSuite");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.TestAttempt", b =>
                {
                    b.HasOne("Application.Features.Tests.Entities.Test", "Test")
                        .WithMany("Attempts")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Test");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.TestSuite", b =>
                {
                    b.HasOne("Application.Features.Github.Entities.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.Repository", b =>
                {
                    b.Navigation("Workflows");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.Workflow", b =>
                {
                    b.Navigation("WorkflowRuns");
                });

            modelBuilder.Entity("Application.Features.Github.Entities.WorkflowRun", b =>
                {
                    b.Navigation("Jobs");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.Test", b =>
                {
                    b.Navigation("Attempts");
                });

            modelBuilder.Entity("Application.Features.Tests.Entities.TestSuite", b =>
                {
                    b.Navigation("Tests");
                });
#pragma warning restore 612, 618
        }
    }
}