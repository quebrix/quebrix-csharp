using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using QuebrixClient;

public class QuebrixBuilder
{
    private readonly IServiceCollection _services;

    public QuebrixBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public QuebrixBuilder WithCache()
    {
        StaticQuebrixConnectionOptions.HasCache = true;
        return this;
    }

    public QuebrixBuilder WithEFSharding<TSharder>() where TSharder : class, IQuebrixEFSharder
    {
        StaticQuebrixConnectionOptions.HasEFSharding = true;
        _services.AddScoped<IQuebrixEFSharder, TSharder>();
        _services.AddScoped<QuebrixShardingConnectionInterceptor>();
        return this;
    }

    public QuebrixBuilder ByQuebrixDbContext<TContext>() where TContext : DbContext
    {
        _services.AddDbContext<TContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<QuebrixShardingConnectionInterceptor>();
            options.UseSqlServer("");
            options.AddInterceptors(interceptor);
        });

        return this;
    }

    public IServiceCollection Build()
    {
        return _services;
    }
}

