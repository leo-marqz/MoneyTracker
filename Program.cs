using System;
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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

var jwtConfig = builder.Configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey));

builder.Services.AddAuthentication((options) =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer((options) =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true; // Save the token in the authentication properties
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Issued by the server
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,

        ValidateIssuer = true,
        ValidIssuer = jwtConfig.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtConfig.Audience,

        ValidateLifetime = true
    };
});


builder.Services.AddRouting((options) => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen((options)=>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MoneyTracker API",
        Version = "v1",
        Description = "API for MoneyTracker application"
    });
});

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
