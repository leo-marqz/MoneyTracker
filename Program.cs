using System;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoneyTracker.Database;
using MoneyTracker.Models;
using MoneyTracker.Services.Email;
using MoneyTracker.Services.JwtToken;

var builder = WebApplication.CreateBuilder(args);

//====================================================================
// Add services to the container.
//====================================================================

builder.Services.AddDbContext<MoneyTrackerDbContext>((options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    #if DEBUG
        options.EnableSensitiveDataLogging();
    #endif
});

builder.Services.AddIdentity<User, Role>((options) =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    
}).AddEntityFrameworkStores<MoneyTrackerDbContext>().AddDefaultTokenProviders();

// FluentValidation setup
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// AutoMapper setup
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Register the EmailService and SMTPConfiguration
builder.Services.Configure<SMTPConfiguration>(builder.Configuration.GetSection("SMTPConfiguration"));
builder.Services.AddTransient<IEmailService, EmailService>();

// Register the JWT token service
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

builder.Services.AddRouting((options) => options.LowercaseUrls = true);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//====================================================================
// Configure the HTTP request pipeline.
//====================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
