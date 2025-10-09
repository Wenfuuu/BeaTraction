using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Interfaces;
using BeaTraction.Infrastructure.Persistence;
using BeaTraction.Infrastructure.Repositories;
using BeaTraction.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaTraction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = BuildConnectionString(configuration);

        // db context
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IAttractionRepository, AttractionRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();

        // services
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
    
    private static string BuildConnectionString(IConfiguration configuration)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "db_name";
        var username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";

        return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    }
}
