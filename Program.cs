//Creates a WebApplicationBuilder which sets up configuration, logging, dependency injection, etc.
//args comes from the command line and can be used for custom configuration.
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Repositories;
using DensityReportingToolBackend.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobService, JobService>();

builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();
builder.Services.AddScoped<IPeopleService, PeopleService>();

// Add services to the container.
//Registers MVC controllers so your API endpoints can respond to requests.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Enables API metadata for minimal APIs, used by Swagger.
builder.Services.AddEndpointsApiExplorer();
// Adds Swagger/OpenAPI generation, so you can view and test your API in /swagger
builder.Services.AddSwaggerGen();

// Add CORS support for local development and production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new List<string>
        {
            "http://localhost:5173",  // Vite dev server
            "http://localhost:3000",  // Create React App dev server
            "https://density-reporting-tool-frontend-2ue6fw1zs.vercel.app",  // Vercel deployment
            "https://drt-ui-lrelh.ondigitalocean.app"  // DigitalOcean deployment
        };

        // Add any additional origins from configuration
        var additionalOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (additionalOrigins != null)
        {
            allowedOrigins.AddRange(additionalOrigins);
        }

        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// get our connection string in appsettings.development.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
// Note: Services (JobService, ProctorService, ReportService) are instantiated directly in their controllers, not via DI

// Returns a WebApplication instance, which represents your running server
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before other middleware
app.UseCors("AllowFrontend");

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
            new ProctorType { Type = "Standard" },
            new ProctorType { Type = "Modified" }
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
        var standardType = await context.ProctorTypes.FirstAsync(pt => pt.Type == "Standard");
        var modifiedType = await context.ProctorTypes.FirstAsync(pt => pt.Type == "Modified");
        
        foreach (var labTest in labTests)
        {
            // Standard Proctor
            proctors.Add(new Proctor
            {
                ProctorID = $"Standard-{labTest.Id}",
                LabTestId = labTest.Id,
                ProctorTypeId = standardType.Id,
                MaxDensity = 1.85,
                CorrectedDensity = 1.82,
                OptimumMoistureContent = 12.5,
                SpecificGravity = 2.65
            });
            
            // Modified Proctor
            proctors.Add(new Proctor
            {
                ProctorID = $"Modified-{labTest.Id}",
                LabTestId = labTest.Id,
                ProctorTypeId = modifiedType.Id,
                MaxDensity = 2.05,
                CorrectedDensity = 2.02,
                OptimumMoistureContent = 10.8,
                SpecificGravity = 2.65
            });
        }
        
        context.Proctors.AddRange(proctors);
        await context.SaveChangesAsync();
    }
}
