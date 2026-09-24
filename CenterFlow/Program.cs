using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Infrastructure.Data;
using CenterFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAppDbContext>(x => x.GetRequiredService<AppDbContext>());
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration["cs"]));
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
