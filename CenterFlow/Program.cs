using CenterFlow;
using CenterFlow.Application;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Data;
using CenterFlow.Infrastructure.Hubs;
using CenterFlow.Infrastructure.Services;
using CenterFlow.Infrastructure.Settings;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAppDbContext>(x => x.GetRequiredService<AppDbContext>());
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration["cs"]));
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICancelEnrollment,CancelEnrollment>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration["hangfire"]));
builder.Services.AddHangfireServer();
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IdentitySeeder>();
builder.Services.AddTransient<GlobalExceptionHandling>();
builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<IJwtService, JwtService>();
var jwt = builder.Configuration.GetSection("JWT").Get<JwtSetting>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
    };
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IMarker).Assembly));
builder.Services.AddScoped<RedLockFactory>(x =>
{
    var cs = builder.Configuration["Redis"];
    var endpoint = new List<RedLockEndPoint>()
    { new DnsEndPoint(cs.Split(':')[0], int.Parse(cs.Split(':')[1])) };
    return RedLockFactory.Create(endpoint);
});
builder.Services.AddScoped<INotificationService, NotificationService>();
var app = builder.Build();

 using (var scope=  app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<IdentitySeeder>().SeedRole();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<GlobalExceptionHandling>();

app.UseHttpsRedirection();

app.UseAuthentication();   
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");
app.MapControllers();
RecurringJob.AddOrUpdate<IMarkCompletedSessionsJob>(
    "mark-completed-sessions",
    x=>x.Execute(),
    Cron.Daily
    );
app.MapHub<NotificationHub>("/hub/notifications");

app.Run();
