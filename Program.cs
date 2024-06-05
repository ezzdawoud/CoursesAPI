using CloudinaryDotNet;
using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Courses.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using SendGrid.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://coursesv3.vercel.app")
                   .AllowAnyHeader()
                   .AllowAnyMethod().AllowAnyOrigin();
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

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "My API",
        Description = "A simple example ASP.NET Core Web API",
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowLocalhost4200");

// Seed initial data if necessary
//Seed.SeedUsersAndRoles(app);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
