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
            builder.RegisterType<TMDBServices>().AsSelf();
            builder.RegisterType<MidiaServices>().AsSelf();

            // Forms
            builder.RegisterType<Home>().AsSelf();

            return builder.Build();
        }
    }
}