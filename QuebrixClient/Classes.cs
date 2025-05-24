using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace QuebrixClient;

public class QuebrixConnectionOptions
{
    /// <summary>
    /// without http or https
    /// </summary>
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

public static class StaticQuebrixConnectionOptions
{
    public static string Host { get; set; } = string.Empty;
    public static int Port { get; set; }
    public static string UserName { get; set; } = string.Empty;
    public static string Password { get; set; } = string.Empty;
    public static bool? HasEFSharding { get; set; }
    public static bool? HasCache { get; set; }
}


public class ShardConfiguration
{
    public int FromKey { get; set; }
    public int ToKey { get; set; }
    public string? ConnectionString { get; set; }
}


public class QuebrixShardingConnectionInterceptor : DbConnectionInterceptor
{
    private readonly IQuebrixCacheProvider _cacheProvider;
    private readonly IQuebrixEFSharder _sharder;

    public QuebrixShardingConnectionInterceptor(
        IQuebrixCacheProvider cacheProvider,
        IQuebrixEFSharder sharder)
    {
        _cacheProvider = cacheProvider;
        _sharder = sharder;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        var shardConfigResult = await _cacheProvider.GetAsync<List<ShardConfiguration>>("sharding", "sharding");
        int? shardKey = await _sharder.GetShardingKey();

        if (shardConfigResult?.Data == null || !shardKey.HasValue)
            throw new InvalidOperationException("Shard config or shard key not available.");

        var matchedShard = shardConfigResult.Data
            .FirstOrDefault(z => z.FromKey <= shardKey && shardKey <= z.ToKey);

        if (matchedShard == null)
            throw new InvalidOperationException($"No shard matched for key: {shardKey}");

        connection.ConnectionString = matchedShard.ConnectionString;

        return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }
}

public class QuebrixDbContext<TContext> : DbContext where TContext : DbContext
{
    private readonly IQuebrixCacheProvider _cacheProvider;
    private readonly IQuebrixEFSharder _sharder;

    public QuebrixDbContext(
        DbContextOptions<TContext> options,
        IQuebrixCacheProvider cacheProvider,
        IQuebrixEFSharder sharder)
        : base(options)
    {
        _cacheProvider = cacheProvider;
        _sharder = sharder;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.AddInterceptors(new QuebrixShardingConnectionInterceptor(_cacheProvider, _sharder));
        }
    }
}




