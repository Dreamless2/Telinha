using Autofac;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Factory;
using Telinha.Services;

namespace Telinha.Container
{
    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();

        // MemoryCache
        builder.Register(c => new MemoryCache(new MemoryCacheOptions()))
          .As<IMemoryCache>()
          .SingleInstance();

        // Cache híbrido
        builder.RegisterType<FileCacheService>()
          .AsSelf()
          .SingleInstance();

        // APIs
        builder.RegisterType<ApiClientFactory>().AsSelf().SingleInstance();
        builder.RegisterType<TMDBServices>().AsSelf();
        builder.RegisterType<MidiaServices>().AsSelf();

        // Forms
        builder.RegisterType<Home>().AsSelf();

        return builder.Build();
    }
}