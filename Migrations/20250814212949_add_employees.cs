using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DensityReportingToolBackend.Migrations
{
    /// <inheritdoc />
    public partial class add_employees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_PersonalInfos_EmployeeId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_PersonalInfos_ProjectManagerId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalInfos_Roles_RoleId",
                table: "PersonalInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_PersonalInfos_EmployeeId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_PersonalInfos_ReviewerId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_PersonalInfos_RoleId",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "PersonalInfos");

            migrationBuilder.CreateTable(
                name: "GeoPacificEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoPacificEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeoPacificEmployees_PersonalInfos_Id",
                        column: x => x.Id,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeoPacificEmployees_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeoPacificEmployees_RoleId",
                table: "GeoPacificEmployees",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_GeoPacificEmployees_EmployeeId",
                table: "Comments",
                column: "EmployeeId",
                principalTable: "GeoPacificEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_GeoPacificEmployees_ProjectManagerId",
                table: "Jobs",
                column: "ProjectManagerId",
                principalTable: "GeoPacificEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_GeoPacificEmployees_EmployeeId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_GeoPacificEmployees_ProjectManagerId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_GeoPacificEmployees_EmployeeId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_GeoPacificEmployees_ReviewerId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "GeoPacificEmployees");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "PersonalInfos",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "PersonalInfos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInfos_RoleId",
                table: "PersonalInfos",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_PersonalInfos_EmployeeId",
                table: "Comments",
                column: "EmployeeId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_PersonalInfos_ProjectManagerId",
                table: "Jobs",
                column: "ProjectManagerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalInfos_Roles_RoleId",
                table: "PersonalInfos",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
