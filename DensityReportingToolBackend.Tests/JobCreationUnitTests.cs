using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using DensityReportingToolBackend.Controllers;
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.Tests;

public class JobCreationUnitTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly JobsController _controller;
    private readonly Mock<ILogger<JobsController>> _mockLogger;

    public JobCreationUnitTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockLogger = new Mock<ILogger<JobsController>>();
        _controller = new JobsController(_mockLogger.Object, _context);
    }

    [Fact]
    public async Task CreateJob_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "TEST-001",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345",
            StartDate = DateTime.UtcNow.AddDays(1),
            JobNotes = "This is a test job note"
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        
        var responseData = createdResult.Value;
        Assert.NotNull(responseData);
    }

    [Fact]
    public async Task CreateJob_WithDuplicateJobNumber_ReturnsBadRequest()
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "DUPLICATE-001",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345"
        };

        // Create the first job
        await _controller.CreateJob(jobRequest);

        // Act - Try to create a job with the same number
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        
        var responseData = badRequestResult.Value;
        Assert.NotNull(responseData);
    }

    [Theory]
    [InlineData("", "Test Client", "Test Project", "123 Test Street", "Job Number is required")]
    [InlineData("TEST-002", "", "Test Project", "123 Test Street", "Client Name is required")]
    [InlineData("TEST-003", "Test Client", "", "123 Test Street", "Project Name is required")]
    [InlineData("TEST-004", "Test Client", "Test Project", "", "Site Address is required")]
    public async Task CreateJob_WithMissingRequiredFields_ReturnsBadRequest(
        string jobNumber, string clientName, string projectName, string siteAddress, string expectedMessage)
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = jobNumber,
            ClientName = clientName,
            ProjectName = projectName,
            SiteAddress = siteAddress
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        
        var responseData = badRequestResult.Value;
        Assert.NotNull(responseData);
    }

    [Fact]
    public async Task CreateJob_WithJobNotes_CreatesJobNote()
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "NOTES-001",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345",
            JobNotes = "This is a test job note with important information"
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        // Verify the job note was created in the database
        var job = await _context.Jobs
            .Include(j => j.JobNotes)
            .FirstOrDefaultAsync(j => j.JobNumber == "NOTES-001");

        Assert.NotNull(job);
        Assert.Single(job.JobNotes);
        Assert.Equal("This is a test job note with important information", job.JobNotes.First().Note);
    }

    [Fact]
    public async Task CreateJob_WithoutJobNotes_DoesNotCreateJobNote()
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "NO-NOTES-001",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345"
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        // Verify no job note was created
        var job = await _context.Jobs
            .Include(j => j.JobNotes)
            .FirstOrDefaultAsync(j => j.JobNumber == "NO-NOTES-001");

        Assert.NotNull(job);
        Assert.Empty(job.JobNotes);
    }

    [Fact]
    public async Task CreateJob_WithAlphanumericJobNumber_WorksCorrectly()
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "2024-A",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345"
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        // Verify the job was created with the correct job number
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobNumber == "2024-A");
        Assert.NotNull(job);
        Assert.Equal("2024-A", job.JobNumber);
    }

    [Fact]
    public async Task CreateJob_WithNumericJobNumber_WorksCorrectly()
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "25482",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345"
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        // Verify the job was created with the correct job number
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobNumber == "25482");
        Assert.NotNull(job);
        Assert.Equal("25482", job.JobNumber);
    }

    [Fact]
    public async Task CreateJob_WithOptionalDates_WorksCorrectly()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(30);
        
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "DATES-001",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345",
            StartDate = startDate,
            EndDate = endDate
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        // Verify the job was created with the correct dates
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobNumber == "DATES-001");
        Assert.NotNull(job);
        Assert.Equal(startDate.Date, job.StartDate?.Date);
        Assert.Equal(endDate.Date, job.EndDate?.Date);
    }

    [Fact]
    public async Task CreateJob_WithNullDates_WorksCorrectly()
    {
        // Arrange
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "NULL-DATES-001",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345",
            StartDate = null,
            EndDate = null
        };

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        // Verify the job was created with null dates
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobNumber == "NULL-DATES-001");
        Assert.NotNull(job);
        Assert.Null(job.StartDate);
        Assert.Null(job.EndDate);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
