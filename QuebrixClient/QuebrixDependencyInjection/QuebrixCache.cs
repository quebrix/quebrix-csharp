using Microsoft.Extensions.DependencyInjection;

namespace QuebrixClient.QuebrixDependencyInjection;

public static class QuebrixServiceCollectionExtensions
{
    public static QuebrixBuilder AddQuebrix(this IServiceCollection services, Action<QuebrixConnectionOptions> configureOptions)
    {
        var options = new QuebrixConnectionOptions();
        configureOptions.Invoke(options);

        ArgumentNullException.ThrowIfNull(options.Host, nameof(options.Host));
        ArgumentNullException.ThrowIfNull(options.Port, nameof(options.Port));
        ArgumentNullException.ThrowIfNull(options.Password, nameof(options.Password));
        ArgumentNullException.ThrowIfNull(options.UserName, nameof(options.UserName));

        StaticQuebrixConnectionOptions.Host = options.Host!;
        StaticQuebrixConnectionOptions.Port = (int)options.Port;
        StaticQuebrixConnectionOptions.UserName = options.UserName;
        StaticQuebrixConnectionOptions.Password = options.Password!;

        services.AddScoped<IQuebrixCacheProvider, QuebrixCacheProvider>();
        return new QuebrixBuilder(services);
    }
}


