using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Wallet.Modules.asset_module;
using Wallet.Modules.position_module;
using Wallet.Modules.trade_module;
using Wallet.Modules.user_module;
using Wallet.Tools.alpha_vantage;
using Wallet.Tools.database;
using Wallet.Tools.scheduler;

#region var
var builder = WebApplication.CreateBuilder(args);
var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1234";
#endregion

#region builder
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();

builder.Services.AddCors(options => { options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });

#region Scheduler
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings();

    config.UsePostgreSqlStorage(connectionString);
});
#endregion

#region DbContext
builder.Services.AddDbContext<Context>(options => options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.RemoteCertificateValidationCallback((_, _, _, _) => true)));
#endregion

#region Dependency Injection
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAlphaVantageService, AlphaVantageService>();
builder.Services.AddScoped<IHangfireSchedulerService, HangfireSchedulerService>();
#endregion

builder.Services.AddEndpointsApiExplorer();

#region Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration["AppSettings:Token"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
#endregion
#endregion

#region app
var app = builder.Build();

app.UseCors("CorsPolicy");

#region Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireServer();

app.UseHangfireDashboard();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var hangfireSchedulerService = serviceProvider.GetRequiredService<IHangfireSchedulerService>();
    hangfireSchedulerService.ScheduleJobs();
}

app.MapControllers();

app.Run();

#endregion
