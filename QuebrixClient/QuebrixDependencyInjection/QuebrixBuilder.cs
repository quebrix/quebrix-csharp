using Microsoft.Extensions.DependencyInjection;
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

    public QuebrixBuilder WithEFSharding()
    {
        StaticQuebrixConnectionOptions.HasEFSharding = true;
        return this;
    }

    public IServiceCollection Build()
    {
        return _services;
    }
}
