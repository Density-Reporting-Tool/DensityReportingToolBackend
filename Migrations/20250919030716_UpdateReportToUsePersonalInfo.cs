using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DensityReportingToolBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportToUsePersonalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_GeoPacificEmployees_EmployeeId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_GeoPacificEmployees_ReviewerId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewerId",
                table: "Reports",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_PersonalInfos_EmployeeId",
                table: "Reports",
                column: "EmployeeId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_PersonalInfos_ReviewerId",
                table: "Reports",
                column: "ReviewerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_PersonalInfos_EmployeeId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_PersonalInfos_ReviewerId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewerId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_GeoPacificEmployees_EmployeeId",
                table: "Reports",
                column: "EmployeeId",
                principalTable: "GeoPacificEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_GeoPacificEmployees_ReviewerId",
                table: "Reports",
                column: "ReviewerId",
                principalTable: "GeoPacificEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
