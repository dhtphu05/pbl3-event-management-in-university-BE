using DUTEventManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using DUTEventManagementAPI.Models;
using System.Text;
using Microsoft.Extensions.Options;
using DUTEventManagementAPI.Services;
using DUTEventManagementAPI.Extensions;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Adð FluentEmail services
var emailSettings = builder.Configuration.GetSection("EmailSettings");
var defaultFromEmail = emailSettings["DefaultSenderEmail"];
var defaultSender = emailSettings["DefaultSender"];
var host = emailSettings["Host"];
var port = emailSettings.GetValue<int>("Port");
var userName = emailSettings["UserName"];
var password = emailSettings["Password"];

Console.WriteLine($"DefaultSenderEmail: {defaultFromEmail}");
Console.WriteLine($"Host: {host}");
Console.WriteLine($"Port: {port}");

builder.Services
    .AddFluentEmail(defaultFromEmail, defaultSender)
    .AddSmtpSender(host,port);

// Add authentication services
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        var clientId = builder.Configuration["Authentication:Google:ClientId"];
        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentNullException("Google ClientId is not configured.");
        }

        var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentNullException("Google ClientSecret is not configured.");
        }
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminPolicy", policy =>
            policy.RequireRole("Admin"));
        options.AddPolicy("OrganizerPolicy", policy =>
            policy.RequireRole("Organizer"));
        options.AddPolicy("UserPolicy", policy =>
            policy.RequireRole("User"));
    });

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IEventCategoryService, EventCategoryService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IEventImageService, EventImageService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:7024", "http://localhost:5044")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();