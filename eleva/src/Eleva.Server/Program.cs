using Eleva.Server.Accessors;
using Eleva.Server.Mcp;
using Eleva.Server.Mcp.Services;
using Eleva.Server.Middleware;
using Eleva.Services.Data;
using Eleva.Services.Services.AICopilot;
using Eleva.Services.Services.Assessments;
using Eleva.Services.Services.Core;
using Eleva.Services.Services.Engagement;
using Eleva.Services.Services.ExecutionStatus;
using Eleva.Services.Services.History;
using Eleva.Services.Services.People;
using Eleva.Services.Services.Pdi;
using Eleva.Services.Services.Performance;
using Eleva.Services.Services.Questions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Data.Sqlite;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Eleva.Server.Jobs;
using Quartz;

// Load .env from solution root
var envFile = Path.Combine(AppContext.BaseDirectory, ".env");
var solutionEnv = FindSolutionEnv(AppContext.BaseDirectory);
if (solutionEnv != null && File.Exists(solutionEnv))
{
    foreach (var line in File.ReadAllLines(solutionEnv))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;
        var idx = line.IndexOf('=');
        if (idx < 0) continue;
        var key = line[..idx].Trim();
        var val = line[(idx + 1)..].Trim();
        if (!string.IsNullOrEmpty(key) && Environment.GetEnvironmentVariable(key) == null)
            Environment.SetEnvironmentVariable(key, val);
    }
}

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
if (connectionString.EndsWith(".db", StringComparison.OrdinalIgnoreCase) || connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
{
    connectionString = NormalizeSqliteConnectionString(connectionString, builder.Environment.ContentRootPath);
    builder.Services.AddDbContext<AppDbContext>(options => options
        .UseSqlite(connectionString)
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));
}
else
{
    var version = builder.Configuration["MySql:ServerVersion"] ?? "11.0.0-mariadb";
    builder.Services.AddDbContext<AppDbContext>(options => options
        .UseMySql(connectionString, ServerVersion.Parse(version), mysql => mysql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null))
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));
}

builder.Services.AddScoped<InstanceContext>();
builder.Services.AddScoped<Eleva.Shared.Interfaces.ICurrentInstanceAccessor, ScopedInstanceAccessor>();
builder.Services.AddSingleton<JobInstanceAccessor>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDiscService, DiscService>();
builder.Services.AddScoped<IEcService, EcService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<IPdiService, PdiService>();
builder.Services.AddScoped<IPdiCycleService, PdiCycleService>();
builder.Services.AddScoped<IBscService, BscService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPulseService, PulseService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddScoped<IQuestionBankService, QuestionBankService>();
builder.Services.AddScoped<IAiRecommendationService, AiRecommendationService>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IExecutionStatusService, ExecutionStatusService>();

builder.Services.AddHttpClient("AiRouter");

builder.Services.AddScoped<IMcpService, PeopleMcpService>();
builder.Services.AddScoped<IMcpService, DiscMcpService>();
builder.Services.AddScoped<IMcpService, EcMcpService>();
builder.Services.AddScoped<IMcpService, PdiMcpService>();
builder.Services.AddScoped<IMcpService, PerformanceMcpService>();
builder.Services.AddScoped<IMcpService, EngagementMcpService>();
builder.Services.AddScoped<IMcpService, AiCopilotMcpService>();
builder.Services.AddScoped<IMcpService, QuestionsMcpService>();
builder.Services.AddScoped<IMcpService, AuthMcpService>();
builder.Services.AddScoped<IMcpService, AuditMcpService>();
builder.Services.AddScoped<IMcpService, ConfigMcpService>();
builder.Services.AddScoped<IMcpService, SystemMcpService>();

builder.Services.AddSingleton<McpServiceRegistry>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddQuartz(q =>
{

    void AddJob<T>(string cronExpression) where T : IJob
    {
        var key = new JobKey(typeof(T).Name);
        q.AddJob<T>(opts => opts.WithIdentity(key));
        q.AddTrigger(opts => opts
            .ForJob(key)
            .WithIdentity($"{typeof(T).Name}-trigger")
            .WithCronSchedule(cronExpression));
    }

    AddJob<PdiNotificationJob>("0 0 8 * * ?");
    AddJob<AssessmentCycleJob>("0 0 9 ? * MON");
    AddJob<PulseSurveyJob>("0 0 10 ? * MON");
    AddJob<PerformanceCycleJob>("0 0 7 1 * ?");
    AddJob<AiRecommendationJob>("0 0 6 * * ?");
    AddJob<AuditConsolidationJob>("0 0 2 * * ?");
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);

var app = builder.Build();

// Register MCP tools
var registry = app.Services.GetRequiredService<McpServiceRegistry>();
using (var scope = app.Services.CreateScope())
{
    var mcpServices = scope.ServiceProvider.GetServices<IMcpService>();
    foreach (var svc in mcpServices)
        svc.RegisterTools(registry);
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (app.Environment.IsEnvironment("Testing"))
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
}

app.UseMiddleware<InstanceContextMiddleware>();
app.UseStaticFiles();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapControllers();
app.Run();

static string NormalizeSqliteConnectionString(string connectionString, string contentRootPath)
{
    var builder = new SqliteConnectionStringBuilder(connectionString);
    if (!Path.IsPathRooted(builder.DataSource))
    {
        var solutionRoot = FindSolutionRoot(contentRootPath);
        if (!string.IsNullOrWhiteSpace(solutionRoot))
        {
            builder.DataSource = Path.GetFullPath(Path.Combine(solutionRoot, builder.DataSource));
        }
    }

    return builder.ToString();
}

static string? FindSolutionEnv(string startPath)
{
    var root = FindSolutionRoot(startPath);
    return root != null ? Path.Combine(root, ".env") : null;
}

static string? FindSolutionRoot(string startPath)
{
    var current = new DirectoryInfo(startPath);
    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, "Eleva.sln")))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    return null;
}

public partial class Program { }
