using CloudinaryDotNet;
using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Courses.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Register custom services
builder.Services.AddScoped<GenarateToken>();

// Configure server options to handle large requests
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // Default is 30 MB
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});

// Configure DbContext with detailed logging for debugging
builder.Services.AddDbContext<Connections>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnections"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

// Configure Identity
builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<Connections>()
    .AddDefaultTokenProviders();

// Configure caching and session
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Session configuration
});

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
});

// Configure email sender service with SendGrid
builder.Services.Configure<EmailSenderService>(builder.Configuration.GetSection("Email"));
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration.GetSection("Email").GetValue<string>("ApiKey");
});
builder.Services.AddScoped<IEmailSender, EmailSenderService>();

// Register IConfiguration as a singleton
var configuration = builder.Configuration; // Capture the IConfiguration instance
builder.Services.AddSingleton<IConfiguration>(configuration);

// Configure Cloudinary
builder.Services.AddSingleton<Cloudinary>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new Cloudinary(new Account(
        config["CloudinarySettings:CloudName"],
        config["CloudinarySettings:ApiKey"],
        config["CloudinarySettings:ApiSecret"]
    ));
});

var app = builder.Build();

// Seed initial data if necessary
//Seed.SeedUsersAndRoles(app);

// Enable CORS
app.UseCors("AllowLocalhost4200");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
