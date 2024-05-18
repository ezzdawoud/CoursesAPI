using CloudinaryDotNet;
using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Courses.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddScoped<GenarateToken>(); // Add this line to register the GenarateToken class
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});

builder.Services.AddDbContext<Connections>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnections")));

builder.Services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<Connections>().AddDefaultTokenProviders();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Session configuration
});
builder.Services.Configure < IdentityOptions > (Options =>
{
    Options.Password.RequireUppercase = false;
    Options.Password.RequiredLength = 8;
}
    );


builder.Services.Configure<EmailSenderService>(builder.Configuration.GetSection("Email"));
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration.GetSection("Email").GetValue<string>("ApiKey");
});
builder.Services.AddScoped<IEmailSender, EmailSenderService>();

var configuration = builder.Configuration; // Capture the IConfiguration instance
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddSingleton<Cloudinary>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new Cloudinary(new Account(
        config["CloudinarySettings:CloudName"],
        config["CloudinarySettings:ApiKey"],
        config["CloudinarySettings:ApiSecret"]
    ));
});

//builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
/*var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();*/

var app = builder.Build();

//Seed.SeedUsersAndRoles(app);

app.UseCors("AllowLocalhost4200");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
