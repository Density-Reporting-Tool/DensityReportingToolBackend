//Creates a WebApplicationBuilder which sets up configuration, logging, dependency injection, etc.
//args comes from the command line and can be used for custom configuration.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Registers MVC controllers so your API endpoints can respond to requests.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Enables API metadata for minimal APIs, used by Swagger.
builder.Services.AddEndpointsApiExplorer();
// Adds Swagger/OpenAPI generation, so you can view and test your API in /swagger
builder.Services.AddSwaggerGen();

// Returns a WebApplication instance, which represents your running server
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Redirects HTTP requests to HTTPS automatically
app.UseHttpsRedirection();

app.UseAuthorization();

// Maps your controllers so they handle incoming requests
app.MapControllers();

// Starts the server
app.Run();
