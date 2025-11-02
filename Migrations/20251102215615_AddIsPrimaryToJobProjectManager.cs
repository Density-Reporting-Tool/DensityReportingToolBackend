using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DensityReportingToolBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPrimaryToJobProjectManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobProjectManagers_JobId",
                table: "JobProjectManagers");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "JobProjectManagers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_JobProjectManagers_JobId_IsPrimary_IsActive",
                table: "JobProjectManagers",
                columns: new[] { "JobId", "IsPrimary", "IsActive" },
                unique: true,
                filter: "\"IsPrimary\" = true AND \"IsActive\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobProjectManagers_JobId_IsPrimary_IsActive",
                table: "JobProjectManagers");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "JobProjectManagers");

            migrationBuilder.CreateIndex(
                name: "IX_JobProjectManagers_JobId",
                table: "JobProjectManagers",
                column: "JobId");
        }
    }
}
