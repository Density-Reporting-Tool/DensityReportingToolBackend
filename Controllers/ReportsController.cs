using DensityReportingToolBackend.Infrastructure;
using DensityReportingToolBackend.Models.DTOs;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController(IReportService reportService, ILogger<ReportsController> logger) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReportListByJobResponse>>>> GetAllReports()
        {
            logger.LogInformation("Retrieving all reports for dashboard");
            var reports = await reportService.GetAllReportsAsync();
            return Success(reports, "Reports retrieved successfully");
        }

        [HttpGet("{reportId:int}")]
        public async Task<ActionResult<ApiResponse<ReportDetailResponse>>> GetReport(int reportId)
        {
            var report = await reportService.GetReportAsync(reportId);

            if (report is null)
            {
                logger.LogWarning("Report {ReportId} was requested but not found", reportId);
                return Failure<ReportDetailResponse>($"Report with ID {reportId} not found", 404);
            }

            return Success(report);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReportDataResponse>>> CreateReport([FromBody] CreateReportRequest request)
        {
            logger.LogInformation("Creating new report for job ID: {JobId}", request.JobId);
            var result = await reportService.CreateReportAsync(request);
            return Created(nameof(GetReport), new { reportId = result.Report.Id }, result.Report);
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReportListByJobResponse>>>> SearchReports(
            [FromQuery] string jobNumber,
            [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return Failure<IEnumerable<ReportListByJobResponse>>("jobNumber query parameter is required");

            var reports = await reportService.SearchReportsByJobNumberAsync(jobNumber, limit);
            return Success(reports, $"Found {reports.Count()} reports matching '{jobNumber}'");
        }

        [HttpGet("job/{jobNumber}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReportListByJobResponse>>>> GetReportsByJob(string jobNumber)
        {
            var reports = await reportService.GetReportsByJobAsync(jobNumber);
            return Success(reports, $"Reports for job {jobNumber} retrieved successfully");
        }

        [HttpGet("proctors/job/{jobId:int}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReportProctorDataResponse>>>> GetProctorsForJob(int jobId)
        {
            var proctors = await reportService.GetProctorsForJobAsync(jobId);
            return Success(proctors, $"Proctors for job ID {jobId} retrieved successfully");
        }

        [HttpPatch("{reportId:int}/memo")]
        public async Task<ActionResult<ApiResponse<MemoInfo>>> UpdateMemo(
            int reportId,
            [FromBody] UpdateMemoRequest request)
        {
            logger.LogInformation("Updating memo for report {ReportId}", reportId);
            var result = await reportService.UpdateMemoAsync(reportId, request);

            if (result is null)
                return Failure<MemoInfo>($"Report with ID {reportId} not found", 404);

            return Success(result, "Memo updated successfully");
        }

        [HttpPost("{reportId:int}/density-test")]
        public async Task<ActionResult<ApiResponse<DensityTestCreateResponse>>> CreateDensityTest(
            int reportId,
            [FromBody] CreateDensityTestRequest request)
        {
            logger.LogInformation("Creating density test for report {ReportId}", reportId);
            var result = await reportService.CreateDensityTestAsync(reportId, request);
            return Success(result, "Density test created successfully");
        }
    }
}
