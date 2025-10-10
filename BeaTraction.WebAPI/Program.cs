using BeaTraction.Application;
using BeaTraction.Application.Interfaces;
using BeaTraction.Infrastructure;
using BeaTraction.Infrastructure.Hubs;
using BeaTraction.Infrastructure.Persistence;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

if (File.Exists(".env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// Debug: Print environment variables
Console.WriteLine("=== Environment Variables ===");
Console.WriteLine($"DB_HOST: {Environment.GetEnvironmentVariable("DB_HOST")}");
Console.WriteLine($"DB_PORT: {Environment.GetEnvironmentVariable("DB_PORT")}");
Console.WriteLine($"POSTGRES_DB: {Environment.GetEnvironmentVariable("POSTGRES_DB")}");
Console.WriteLine($"POSTGRES_USER: {Environment.GetEnvironmentVariable("POSTGRES_USER")}");
Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
Console.WriteLine("=============================");

// Add Application layer (MediatR, FluentValidation)
builder.Services.AddApplication();

// Add Infrastructure layer (DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Add JWT Authentication
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") 
    ?? builder.Configuration["Jwt:Secret"] 
    ?? throw new InvalidOperationException("JWT Secret is not configured");

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
    ?? builder.Configuration["Jwt:Issuer"] 
    ?? "BeaTraction";

var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
    ?? builder.Configuration["Jwt:Audience"] 
    ?? "BeaTractionAPI";

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
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (string.IsNullOrEmpty(context.Token))
            {
                context.Token = context.Request.Cookies["authToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Add Controllers
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BeaTraction API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    // Initialize MinIO bucket
    var minioService = scope.ServiceProvider.GetRequiredService<IMinioService>();
    await minioService.EnsureBucketExistsAsync();
    Console.WriteLine("MinIO bucket initialized successfully");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AttractionsHub>("/hubs/attractions");

app.Run();