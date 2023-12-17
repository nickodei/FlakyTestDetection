using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationshiptoRepositoryAndWorkflowRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "WorkflowRunId",
                table: "WorkflowRuns",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "RepositoryId",
                table: "WorkflowRuns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRuns_RepositoryId",
                table: "WorkflowRuns",
                column: "RepositoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowRuns_Repositories_RepositoryId",
                table: "WorkflowRuns",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowRuns_Repositories_RepositoryId",
                table: "WorkflowRuns");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowRuns_RepositoryId",
                table: "WorkflowRuns");

            migrationBuilder.DropColumn(
                name: "RepositoryId",
                table: "WorkflowRuns");

            migrationBuilder.AlterColumn<int>(
                name: "WorkflowRunId",
                table: "WorkflowRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
