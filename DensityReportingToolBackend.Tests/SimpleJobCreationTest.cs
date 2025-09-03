using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using DensityReportingToolBackend.Controllers;
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.Tests;

/// <summary>
/// Simple example of how to test job creation functionality
/// This demonstrates the basic structure and approach for testing your JobsController
/// </summary>
public class SimpleJobCreationTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly JobsController _controller;
    private readonly Mock<ILogger<JobsController>> _mockLogger;

    public SimpleJobCreationTest()
    {
        // Setup in-memory database for testing
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
        // Arrange - Set up test data
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "25000-A",
            ClientName = "GeoPacific Construction",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345",
            StartDate = DateTime.UtcNow.AddDays(1),
            JobNotes = "This is a test job note"
        };

        // Act - Call the method being tested
        var result = await _controller.CreateJob(jobRequest);

        // Assert - Verify the expected outcome
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        
        var responseData = createdResult.Value;
        Assert.NotNull(responseData);
    }

    [Fact]
    public async Task CreateJob_WithDuplicateJobNumber_ReturnsBadRequest()
    {
        // Arrange - Create first job
        var jobRequest = new CreateJobRequest
        {
            JobNumber = "DUPLICATE-001",
            ClientName = "Test Client",
            ProjectName = "Test Project",
            SiteAddress = "123 Test Street, Test City, TC 12345"
        };

        await _controller.CreateJob(jobRequest);

        // Act - Try to create job with same number
        var result = await _controller.CreateJob(jobRequest);

        // Assert - Should return bad request
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
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

    public void Dispose()
    {
        _context.Dispose();
    }
}
