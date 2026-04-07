using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DensityReportingToolBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddProctorAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateSampled",
                table: "Proctors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTested",
                table: "Proctors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabLocation",
                table: "Proctors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialType",
                table: "Proctors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OversizePercentage",
                table: "Proctors",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProctorTestNumber",
                table: "Proctors",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSampled",
                table: "Proctors");

            migrationBuilder.DropColumn(
                name: "DateTested",
                table: "Proctors");

            migrationBuilder.DropColumn(
                name: "LabLocation",
                table: "Proctors");

            migrationBuilder.DropColumn(
                name: "MaterialType",
                table: "Proctors");

            migrationBuilder.DropColumn(
                name: "OversizePercentage",
                table: "Proctors");

            migrationBuilder.DropColumn(
                name: "ProctorTestNumber",
                table: "Proctors");
        }
    }
}
