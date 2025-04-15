using api.Application.Services;
using api.Core.Interfaces;
using api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
    
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MasterConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MasterConnection"))
    )
);

// Register repositories and services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<SaasSetupService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

// Register a tenant provider and tenant DbContext factory for per-client database resolution
builder.Services.AddScoped<TenantProvider>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<TenantDbContextFactory>();
builder.Services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
builder.Services.AddHttpContextAccessor();

// Add controllers
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Database Setup API", 
        Version = "v1",
        Description = "API for setting up various database types",
        Contact = new OpenApiContact
        {
            Name = "RCode Technologies",
            Email = "support@rcode.dev",
            Url = new Uri("https://rcode.dev")
        }
    });
    
    // Enable annotations
    c.EnableAnnotations();
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Database Setup API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Database Setup API Documentation";
    c.DefaultModelsExpandDepth(-1); // Hide models by default
});

// Use CORS
app.UseCors("AllowFrontend");

// Request logging
app.UseSerilogRequestLogging();

app.UseAuthorization();
app.MapControllers();

app.Run();