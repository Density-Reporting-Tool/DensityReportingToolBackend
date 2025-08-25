//Creates a WebApplicationBuilder which sets up configuration, logging, dependency injection, etc.
//args comes from the command line and can be used for custom configuration.
using DensityReportingToolBackend.Data;
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

// Starts the server
app.Run();
