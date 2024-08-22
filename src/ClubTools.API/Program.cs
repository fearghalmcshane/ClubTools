using Microsoft.EntityFrameworkCore;
using ClubTools.Api.Database;
using Carter;
using FluentValidation;
using ClubTools.Api.Extensions;
using Serilog;
using Asp.Versioning;
using Asp.Versioning.Builder;
using ClubTools.Api.OpenApi;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}"));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddApiEndpoints();

//Add a CORS policy for the client
//builder.Services.AddCors(
//    options => options.AddPolicy(
//        "wasm",
//       policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:5001",
//            builder.Configuration["FrontendUrl"] ?? "https://localhost:5002"])
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .AllowCredentials()));
builder.Services.AddCors(
    options => options.AddPolicy(
        "AllowBlazorClient",
       policy => policy.WithOrigins("https://localhost:7020")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddDbContext<IdentityDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

builder.Services.AddCarter();

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Use the CORS policy
app.UseCors("AllowBlazorClient");

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .HasApiVersion(new ApiVersion(2))
            .ReportApiVersions()
            .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{apiVersion:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

versionedGroup.MapCarter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

        foreach (ApiVersionDescription description in descriptions)
        {
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name = description.GroupName.ToUpperInvariant();

            options.SwaggerEndpoint(url, name);
        }
    });

    app.ApplyIdentityMigrations();
    app.ApplyMigrations();
}

//app.UseHttpsRedirection(); Broken the blazor connection. Maybe CORS? To be looked into...

app.MapIdentityApi<User>();

app.Run();

public partial class Program { }
