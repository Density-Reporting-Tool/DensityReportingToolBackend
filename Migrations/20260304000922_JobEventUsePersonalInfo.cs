using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DensityReportingToolBackend.Migrations
{
    /// <inheritdoc />
    public partial class JobEventUsePersonalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add new column (nullable for backfill)
            migrationBuilder.AddColumn<int>(
                name: "PersonalInfoId",
                table: "JobEvents",
                type: "integer",
                nullable: true);

            // 2. Backfill: set PersonalInfoId from GeoPacificEmployees.PersonalInfoId
            migrationBuilder.Sql(@"
                UPDATE ""JobEvents""
                SET ""PersonalInfoId"" = ""GeoPacificEmployees"".""PersonalInfoId""
                FROM ""GeoPacificEmployees""
                WHERE ""JobEvents"".""GeoPacificEmployeeId"" = ""GeoPacificEmployees"".""Id""");

            // 3. Make PersonalInfoId required (fails if any row has no matching employee)
            migrationBuilder.AlterColumn<int>(
                name: "PersonalInfoId",
                table: "JobEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // 4. Drop old FK and index, then drop old column
            migrationBuilder.DropForeignKey(
                name: "FK_JobEvents_GeoPacificEmployees_GeoPacificEmployeeId",
                table: "JobEvents");

            migrationBuilder.DropIndex(
                name: "IX_JobEvents_GeoPacificEmployeeId",
                table: "JobEvents");

            migrationBuilder.DropColumn(
                name: "GeoPacificEmployeeId",
                table: "JobEvents");

            // 5. Add index and FK for new column
            migrationBuilder.CreateIndex(
                name: "IX_JobEvents_PersonalInfoId",
                table: "JobEvents",
                column: "PersonalInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobEvents_PersonalInfos_PersonalInfoId",
                table: "JobEvents",
                column: "PersonalInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobEvents_PersonalInfos_PersonalInfoId",
                table: "JobEvents");

            migrationBuilder.DropIndex(
                name: "IX_JobEvents_PersonalInfoId",
                table: "JobEvents");

            migrationBuilder.AddColumn<int>(
                name: "GeoPacificEmployeeId",
                table: "JobEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""JobEvents""
                SET ""GeoPacificEmployeeId"" = ""GeoPacificEmployees"".""Id""
                FROM ""GeoPacificEmployees""
                WHERE ""JobEvents"".""PersonalInfoId"" = ""GeoPacificEmployees"".""PersonalInfoId""");

            migrationBuilder.AlterColumn<int>(
                name: "GeoPacificEmployeeId",
                table: "JobEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobEvents_GeoPacificEmployeeId",
                table: "JobEvents",
                column: "GeoPacificEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobEvents_GeoPacificEmployees_GeoPacificEmployeeId",
                table: "JobEvents",
                column: "GeoPacificEmployeeId",
                principalTable: "GeoPacificEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "PersonalInfoId",
                table: "JobEvents");
        }
    }
}
