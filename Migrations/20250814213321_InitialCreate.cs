using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DensityReportingToolBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonalInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProctorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProctorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleTitle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contractors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DetailsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contractors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contractors_PersonalInfos_DetailsId",
                        column: x => x.DetailsId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_GeoPacificEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "GeoPacificEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectManagerId = table.Column<int>(type: "integer", nullable: false),
                    ClientName = table.Column<string>(type: "text", nullable: false),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    SiteAddress = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_GeoPacificEmployees_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "GeoPacificEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "JobNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobNotes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobNotes_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    MaterialType = table.Column<string>(type: "text", nullable: true),
                    ImportLocation = table.Column<string>(type: "text", nullable: true),
                    ReceiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTests_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    ReviewerId = table.Column<int>(type: "integer", nullable: false),
                    ReportNumber = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubmitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DistributeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_GeoPacificEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "GeoPacificEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_GeoPacificEmployees_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "GeoPacificEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitePlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    SitePlanUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitePlans_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sieves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabTestId = table.Column<int>(type: "integer", nullable: false),
                    TotalDryMassGrams = table.Column<double>(type: "double precision", nullable: true),
                    MoistureContentBefore = table.Column<double>(type: "double precision", nullable: true),
                    MoistureContentAfter = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sieves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sieves_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportMemos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReportId = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    CommentsAndObservations = table.Column<string>(type: "text", nullable: true),
                    Conclusion = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportMemos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportMemos_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReportId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    GpsAccuracyMeters = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportPhotos_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProctorID = table.Column<string>(type: "text", nullable: true),
                    LabTestId = table.Column<int>(type: "integer", nullable: false),
                    SieveId = table.Column<int>(type: "integer", nullable: true),
                    ProctorTypeId = table.Column<int>(type: "integer", nullable: false),
                    MaxDensity = table.Column<double>(type: "double precision", nullable: true),
                    CorrectedDensity = table.Column<double>(type: "double precision", nullable: true),
                    OptimumMoistureContent = table.Column<double>(type: "double precision", nullable: true),
                    SpecificGravity = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proctors_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proctors_ProctorTypes_ProctorTypeId",
                        column: x => x.ProctorTypeId,
                        principalTable: "ProctorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proctors_Sieves_SieveId",
                        column: x => x.SieveId,
                        principalTable: "Sieves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SieveResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SieveId = table.Column<int>(type: "integer", nullable: false),
                    SieveSize = table.Column<int>(type: "integer", nullable: false),
                    GramsRetained = table.Column<double>(type: "double precision", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SieveResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SieveResults_Sieves_SieveId",
                        column: x => x.SieveId,
                        principalTable: "Sieves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemoComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    MemoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemoComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemoComments_ReportMemos_MemoId",
                        column: x => x.MemoId,
                        principalTable: "ReportMemos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DensityTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProctorId = table.Column<int>(type: "integer", nullable: false),
                    ReportId = table.Column<int>(type: "integer", nullable: false),
                    TestArea = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    ElevationReference = table.Column<int>(type: "integer", nullable: true),
                    ElevationValue = table.Column<double>(type: "double precision", nullable: true),
                    ElevationUnit = table.Column<int>(type: "integer", nullable: true),
                    CorrectedOversizePercentage = table.Column<float>(type: "real", nullable: true),
                    ProbeDepth = table.Column<int>(type: "integer", nullable: true),
                    ProbeDepthUnit = table.Column<int>(type: "integer", nullable: true),
                    CompactionSpecification = table.Column<int>(type: "integer", nullable: true),
                    CompactionSpecificationUnit = table.Column<int>(type: "integer", nullable: true),
                    DensityValue = table.Column<int>(type: "integer", nullable: true),
                    MoistureValue = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "timezone('utc', now())"),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DensityTests", x => x.Id);
                    table.CheckConstraint("ck_densitytest_compactionspec_0_110", "\"CompactionSpecification\" IS NULL OR (\"CompactionSpecification\" >= 0 AND \"CompactionSpecification\" <= 110)");
                    table.CheckConstraint("ck_densitytest_moisture_0_100", "\"MoistureValue\" IS NULL OR (\"MoistureValue\" >= 0 AND \"MoistureValue\" <= 100)");
                    table.CheckConstraint("ck_densitytest_oversize_0_100", "\"CorrectedOversizePercentage\" IS NULL OR (\"CorrectedOversizePercentage\" >= 0 AND \"CorrectedOversizePercentage\" <= 100)");
                    table.ForeignKey(
                        name: "FK_DensityTests_Proctors_ProctorId",
                        column: x => x.ProctorId,
                        principalTable: "Proctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DensityTests_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProctorAdditionalJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProctorId = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProctorAdditionalJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProctorAdditionalJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProctorAdditionalJobs_Proctors_ProctorId",
                        column: x => x.ProctorId,
                        principalTable: "Proctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DensityTestComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    DensityTestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DensityTestComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DensityTestComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DensityTestComments_DensityTests_DensityTestId",
                        column: x => x.DensityTestId,
                        principalTable: "DensityTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShotPlacements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SitePlanId = table.Column<int>(type: "integer", nullable: false),
                    DensityTestId = table.Column<int>(type: "integer", nullable: false),
                    XCoordinate = table.Column<double>(type: "double precision", nullable: false),
                    YCoordinate = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShotPlacements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShotPlacements_DensityTests_DensityTestId",
                        column: x => x.DensityTestId,
                        principalTable: "DensityTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShotPlacements_SitePlans_SitePlanId",
                        column: x => x.SitePlanId,
                        principalTable: "SitePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_EmployeeId",
                table: "Comments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_DetailsId",
                table: "Contractors",
                column: "DetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_DensityTestComments_CommentId",
                table: "DensityTestComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_DensityTestComments_DensityTestId",
                table: "DensityTestComments",
                column: "DensityTestId");

            migrationBuilder.CreateIndex(
                name: "IX_DensityTests_ProctorId",
                table: "DensityTests",
                column: "ProctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DensityTests_ReportId",
                table: "DensityTests",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoPacificEmployees_RoleId",
                table: "GeoPacificEmployees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_JobContractors_ContractorId",
                table: "JobContractors",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_JobNotes_CommentId",
                table: "JobNotes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobNotes_JobId",
                table: "JobNotes",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ProjectManagerId",
                table: "Jobs",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_JobId",
                table: "LabTests",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoComments_CommentId",
                table: "MemoComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoComments_MemoId",
                table: "MemoComments",
                column: "MemoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProctorAdditionalJobs_JobId",
                table: "ProctorAdditionalJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ProctorAdditionalJobs_ProctorId",
                table: "ProctorAdditionalJobs",
                column: "ProctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Proctors_LabTestId",
                table: "Proctors",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_Proctors_ProctorTypeId",
                table: "Proctors",
                column: "ProctorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Proctors_SieveId",
                table: "Proctors",
                column: "SieveId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportMemos_ReportId",
                table: "ReportMemos",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPhotos_ReportId",
                table: "ReportPhotos",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_EmployeeId",
                table: "Reports",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_JobId",
                table: "Reports",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReviewerId",
                table: "Reports",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShotPlacements_DensityTestId",
                table: "ShotPlacements",
                column: "DensityTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShotPlacements_SitePlanId",
                table: "ShotPlacements",
                column: "SitePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SieveResults_SieveId",
                table: "SieveResults",
                column: "SieveId");

            migrationBuilder.CreateIndex(
                name: "IX_Sieves_LabTestId",
                table: "Sieves",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_SitePlans_JobId",
                table: "SitePlans",
                column: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DensityTestComments");

            migrationBuilder.DropTable(
                name: "JobContractors");

            migrationBuilder.DropTable(
                name: "JobNotes");

            migrationBuilder.DropTable(
                name: "MemoComments");

            migrationBuilder.DropTable(
                name: "ProctorAdditionalJobs");

            migrationBuilder.DropTable(
                name: "ReportPhotos");

            migrationBuilder.DropTable(
                name: "ShotPlacements");

            migrationBuilder.DropTable(
                name: "SieveResults");

            migrationBuilder.DropTable(
                name: "Contractors");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ReportMemos");

            migrationBuilder.DropTable(
                name: "DensityTests");

            migrationBuilder.DropTable(
                name: "SitePlans");

            migrationBuilder.DropTable(
                name: "Proctors");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "ProctorTypes");

            migrationBuilder.DropTable(
                name: "Sieves");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "GeoPacificEmployees");

            migrationBuilder.DropTable(
                name: "PersonalInfos");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
