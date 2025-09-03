using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DensityReportingToolBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithUpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobNotes_Comments_CommentId",
                table: "JobNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_JobProjectManagers_GeoPacificEmployees_EmployeeId",
                table: "JobProjectManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Clients_ClientId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "JobContractors");

            migrationBuilder.DropTable(
                name: "Contractors");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ClientId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "JobSiteContacts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "JobSiteContacts");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "JobProjectManagers",
                newName: "PersonalInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_JobProjectManagers_EmployeeId",
                table: "JobProjectManagers",
                newName: "IX_JobProjectManagers_PersonalInfoId");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "PersonalInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobNumber",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "JobNotes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "JobNotes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "JobNotes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_JobNotes_Comments_CommentId",
                table: "JobNotes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobProjectManagers_PersonalInfos_PersonalInfoId",
                table: "JobProjectManagers",
                column: "PersonalInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobNotes_Comments_CommentId",
                table: "JobNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_JobProjectManagers_PersonalInfos_PersonalInfoId",
                table: "JobProjectManagers");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobNumber",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "JobNotes");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "JobNotes");

            migrationBuilder.RenameColumn(
                name: "PersonalInfoId",
                table: "JobProjectManagers",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_JobProjectManagers_PersonalInfoId",
                table: "JobProjectManagers",
                newName: "IX_JobProjectManagers_EmployeeId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "JobSiteContacts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "JobSiteContacts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "JobNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contractors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: true),
                    PersonalInfoId = table.Column<int>(type: "integer", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contractors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contractors_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contractors_PersonalInfos_PersonalInfoId",
                        column: x => x.PersonalInfoId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobContractors",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    ContractorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobContractors", x => new { x.JobId, x.ContractorId });
                    table.ForeignKey(
                        name: "FK_JobContractors_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobContractors_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ClientId",
                table: "Jobs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_ClientId",
                table: "Contractors",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_PersonalInfoId",
                table: "Contractors",
                column: "PersonalInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobContractors_ContractorId",
                table: "JobContractors",
                column: "ContractorId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobNotes_Comments_CommentId",
                table: "JobNotes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobProjectManagers_GeoPacificEmployees_EmployeeId",
                table: "JobProjectManagers",
                column: "EmployeeId",
                principalTable: "GeoPacificEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Clients_ClientId",
                table: "Jobs",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
