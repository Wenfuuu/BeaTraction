using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Interfaces;
using BeaTraction.Infrastructure.Persistence;
using BeaTraction.Infrastructure.Repositories;
using BeaTraction.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BeaTraction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        var connectionString = BuildConnectionString(configuration);

        // db context
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Redis connection with proper retry and timeout settings
        var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
        var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
        var redisConnectionString = $"{redisHost}:{redisPort}";
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            try
            {
                var config = ConfigurationOptions.Parse(redisConnectionString);
                
                // Generous timeouts for Docker networking
                config.AbortOnConnectFail = false; // Don't throw on initial connect failure
                config.ConnectTimeout = 10000; // 10 seconds for initial connection
                config.SyncTimeout = 5000; // 5 seconds for operations
                config.ConnectRetry = 3; // Retry 3 times
                config.KeepAlive = 60; // Keep connection alive
                config.AsyncTimeout = 5000; // 5 seconds for async operations
                
                Console.WriteLine($"Attempting to connect to Redis at {redisConnectionString}...");
                var multiplexer = ConnectionMultiplexer.Connect(config);
                
                // Log connection events
                multiplexer.ConnectionFailed += (sender, args) =>
                {
                    Console.WriteLine($"Redis connection failed: {args.Exception?.Message ?? args.FailureType.ToString()}");
                };
                
                multiplexer.ConnectionRestored += (sender, args) =>
                {
                    Console.WriteLine("Redis connection restored");
                };
                
                if (multiplexer.IsConnected)
                {
                    Console.WriteLine("✅ Redis connection established successfully");
                }
                else
                {
                    Console.WriteLine("⚠️ Redis connection not established immediately, will retry in background");
                }
                
                return multiplexer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to connect to Redis: {ex.Message}");
                Console.WriteLine("⚠️ Application will run without cache. Redis features disabled.");
                
                // Return a non-connected multiplexer that can retry later
                var config = ConfigurationOptions.Parse(redisConnectionString);
                config.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(config);
            }
        });

        // repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IAttractionRepository, AttractionRepository>();
        services.AddScoped<IScheduleAttractionRepository, ScheduleAttractionRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();

        // services
        services.AddScoped<IJwtService, JwtService>();
        services.AddSingleton<IMinioService, MinioService>();
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddScoped<IRealtimeNotificationService, SignalRNotificationService>();

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
