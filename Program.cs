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

// Add CORS to allow frontend connections
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "https://localhost:3000",
                "http://localhost:5173",  // Vite default port
                "https://localhost:5173"  // Vite HTTPS
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    
    // Add policy for production frontend
    options.AddPolicy("AllowProduction", policy =>
    {
        policy.WithOrigins(
                "https://density-reporting-tool-frontend-42lpottl6.vercel.app",  // Vercel
                "https://density-reporting-tool-frontend.vercel.app"  // Alternative Vercel URL
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Add a more permissive policy for testing
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// get our connection string in appsettings.json or environment variables
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

// Only use HTTPS redirection in development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Use CORS before authorization and routing
// For development, use AllowFrontend policy
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowFrontend");
}
// For production, use AllowProduction policy
else if (app.Environment.IsProduction())
{
    app.UseCors("AllowProduction");
}
// Fallback to AllowAll for any other environment (like Render staging)
else
{
    app.UseCors("AllowAll");
}

app.UseAuthorization();

// Maps your controllers so they handle incoming requests
app.MapControllers();

// Starts the server
app.Run();
