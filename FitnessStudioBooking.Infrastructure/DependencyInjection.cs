using FitnessStudioBooking.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FitnessStudioBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=fitness_studio_booking;User=root;Password=Password123!;";

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
        services.AddSingleton<IRedisLock, RedisLock>();

        return services;
    }
}

public sealed class RedisLock(IConnectionMultiplexer connectionMultiplexer) : IRedisLock
{
    public async Task<IAsyncDisposable?> AcquireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken)
    {
        var database = connectionMultiplexer.GetDatabase();
        var lockKey = $"lock:{key}";
        var token = Guid.NewGuid().ToString("N");

        for (var attempt = 0; attempt < 5; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await database.StringSetAsync(lockKey, token, expiry, When.NotExists))
            {
                return new RedisLockHandle(database, lockKey, token);
            }

            await Task.Delay(100, cancellationToken);
        }

        return null;
    }

    private sealed class RedisLockHandle(IDatabase database, string lockKey, string token) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            const string script = """
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end
                """;

            await database.ScriptEvaluateAsync(script, [lockKey], [token]);
        }
    }
}
