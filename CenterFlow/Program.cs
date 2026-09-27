using CenterFlow;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Data;
using CenterFlow.Infrastructure.Services;
using CenterFlow.Infrastructure.Settings;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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
builder.Services.AddScoped<RedLockFactory>(x =>
{
    var cs = builder.Configuration["Redis"];
    var endpoint = new List<RedLockEndPoint>()
    { new DnsEndPoint(cs.Split(':')[0], int.Parse(cs.Split(':')[1])) };
    return RedLockFactory.Create(endpoint);
});
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

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");
app.MapControllers();
RecurringJob.AddOrUpdate<IMarkCompletedSessionsJob>(
    "mark-completed-sessions",
    x=>x.Execute(),
    Cron.Daily
    );

app.Run();
