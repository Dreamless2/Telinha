using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Telinha.Factory;
using Telinha.Services;

namespace Telinha.Container
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            // MemoryCache
            builder.Register(c => new MemoryCache(new MemoryCacheOptions()))
            .As<IMemoryCache>()
            .SingleInstance();

            // Cache híbrido
            builder.RegisterType<FileCacheServices>()
            .AsSelf()
            .SingleInstance();

            // APIs
            builder.RegisterType<ApiClientFactory>().AsSelf().SingleInstance();
            builder.RegisterType<AppConfigServices>().AsSelf().SingleInstance();
            // 🔥 TMDBServices com factory
            builder.Register(c =>
            {
                var factory = c.Resolve<ApiClientFactory>();
                var config = c.Resolve<AppConfigServices>().Load();

                if (string.IsNullOrWhiteSpace(config.TMDB))
                    throw new InvalidOperationException("Token TMDB não encontrado. Configure na tela de Token.");

                var client = factory.GetTMDB(); // RestClient base
                return new TMDBServices(client, config.TMDB); // passa os 2 params
            })
           .AsSelf()
           .SingleInstance(); // TMDBServices pode ser singleton
            builder.RegisterType<MidiaServices>().AsSelf();

            // Forms
            builder.RegisterType<Home>().AsSelf();

            return builder.Build();
        }
    }
}