//Creates a WebApplicationBuilder which sets up configuration, logging, dependency injection, etc.
//args comes from the command line and can be used for custom configuration.
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Registers MVC controllers so your API endpoints can respond to requests.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Enables API metadata for minimal APIs, used by Swagger.
builder.Services.AddEndpointsApiExplorer();
// Adds Swagger/OpenAPI generation, so you can view and test your API in /swagger
builder.Services.AddSwaggerGen();

// Add CORS support for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// get our connection string in appsettings.development.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<DensityReportingToolBackend.Services.IProctorService, DensityReportingToolBackend.Services.ProctorService>();
builder.Services.AddScoped<DensityReportingToolBackend.Services.IReportService, DensityReportingToolBackend.Services.ReportService>();

// Returns a WebApplication instance, which represents your running server
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before other middleware
app.UseCors("AllowLocalFrontend");

// Redirects HTTP requests to HTTPS automatically (disabled in development)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// Maps your controllers so they handle incoming requests
app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData(context);
}

// Starts the server
app.Run();

static async Task SeedData(AppDbContext context)
{
    // Check if proctor types already exist
    if (!await context.ProctorTypes.AnyAsync())
    {
        var proctorTypes = new[]
        {
            new ProctorType { Type = "ASTM D698 - Standard Proctor" },
            new ProctorType { Type = "ASTM D1557 - Modified Proctor" },
            new ProctorType { Type = "AASHTO T99 - Standard Proctor" },
            new ProctorType { Type = "AASHTO T180 - Modified Proctor" }
        };
        
        context.ProctorTypes.AddRange(proctorTypes);
        await context.SaveChangesAsync();
    }

    // Check if we have any jobs to create lab tests for
    var jobs = await context.Jobs.Take(3).ToListAsync();
    if (jobs.Any() && !await context.LabTests.AnyAsync())
    {
        var labTests = new List<LabTest>();
        var proctors = new List<Proctor>();
        
        foreach (var job in jobs)
        {
            var labTest = new LabTest
            {
                JobId = job.Id,
                MaterialType = "Riversand",
                ImportLocation = "Test Location",
                ReceiveDate = DateTime.UtcNow.AddDays(-7)
            };
            labTests.Add(labTest);
        }
        
        context.LabTests.AddRange(labTests);
        await context.SaveChangesAsync();
        
        // Create proctors for each lab test
        var astmStandardType = await context.ProctorTypes.FirstAsync(pt => pt.Type == "ASTM D698 - Standard Proctor");
        var astmModifiedType = await context.ProctorTypes.FirstAsync(pt => pt.Type == "ASTM D1557 - Modified Proctor");
        var aashtoStandardType = await context.ProctorTypes.FirstAsync(pt => pt.Type == "AASHTO T99 - Standard Proctor");
        var aashtoModifiedType = await context.ProctorTypes.FirstAsync(pt => pt.Type == "AASHTO T180 - Modified Proctor");
        
        foreach (var labTest in labTests)
        {
            // ASTM D698 - Standard Proctor
            proctors.Add(new Proctor
            {
                ProctorID = $"ASTM-D698-{labTest.Id}",
                LabTestId = labTest.Id,
                ProctorTypeId = astmStandardType.Id,
                MaxDensity = 1.85,
                CorrectedDensity = 1.82,
                OptimumMoistureContent = 12.5,
                SpecificGravity = 2.65
            });
            
            // ASTM D1557 - Modified Proctor
            proctors.Add(new Proctor
            {
                ProctorID = $"ASTM-D1557-{labTest.Id}",
                LabTestId = labTest.Id,
                ProctorTypeId = astmModifiedType.Id,
                MaxDensity = 2.05,
                CorrectedDensity = 2.02,
                OptimumMoistureContent = 10.8,
                SpecificGravity = 2.65
            });
            
            // AASHTO T99 - Standard Proctor
            proctors.Add(new Proctor
            {
                ProctorID = $"AASHTO-T99-{labTest.Id}",
                LabTestId = labTest.Id,
                ProctorTypeId = aashtoStandardType.Id,
                MaxDensity = 1.83,
                CorrectedDensity = 1.80,
                OptimumMoistureContent = 13.2,
                SpecificGravity = 2.67
            });
            
            // AASHTO T180 - Modified Proctor
            proctors.Add(new Proctor
            {
                ProctorID = $"AASHTO-T180-{labTest.Id}",
                LabTestId = labTest.Id,
                ProctorTypeId = aashtoModifiedType.Id,
                MaxDensity = 2.08,
                CorrectedDensity = 2.05,
                OptimumMoistureContent = 11.2,
                SpecificGravity = 2.67
            });
        }
        
        context.Proctors.AddRange(proctors);
        await context.SaveChangesAsync();
    }
}
