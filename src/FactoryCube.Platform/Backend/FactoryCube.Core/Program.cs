using FactoryCube.Core.Application.Interfaces;
using FactoryCube.Core.Application.Services;
using FactoryCube.Core.Domain.Interfaces;
using FactoryCube.Core.Infrastructure.Data;
using FactoryCube.Core.Infrastructure.Data.Repositories;
using FactoryCube.Core.Infrastructure.Jobs;
using FactoryCube.Core.Infrastructure.PythonRunner;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// DB
builder.Services.AddDbContext<FactoryCubeDbContext>(options =>
    options.UseInMemoryDatabase("FactoryCube"));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
builder.Services.AddScoped<ISyntheticJobRepository, SyntheticJobRepository>();
builder.Services.AddScoped<IMlExperimentRepository, MlExperimentRepository>();

// Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IDatasetService, DatasetService>();
builder.Services.AddScoped<ISyntheticService, SyntheticService>();
builder.Services.AddScoped<IMlService, MlService>();
builder.Services.AddScoped<IQualityService, QualityService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Python Runner & Background Jobs
builder.Services.AddSingleton<PythonRunnerService>();
builder.Services.AddHostedService<SyntheticJobBackgroundService>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Ensure DB created (MVP 단계)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FactoryCubeDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
