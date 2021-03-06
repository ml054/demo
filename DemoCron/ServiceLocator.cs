﻿using System;
using System.IO;
using DemoCommon.Utils.Database;
using DemoCron.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DemoCron
{
    internal static class ServiceLocator
    {
        private static ServiceProvider Provider { get; set; }

        public static T Resolve<T>() => (T)Provider.GetService(typeof(T));

        public static object Resolve(Type t)
        {
            return Provider.GetService(t);
        }

        public static void Configure()
        {
            var serviceCollection = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();


            var loggerFactory = new LoggerFactory()
                .AddNLog();

            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();
            serviceCollection.AddOptions();

            var settings = new Settings();
            configuration.Bind(settings);
            serviceCollection.AddSingleton(settings);

            serviceCollection.AddSingleton<Startup>();
            serviceCollection.AddSingleton<DocumentStoreHolder>();
            serviceCollection.AddSingleton(_ => new DatabaseName(settings.Database, conferenceMode: false));

            serviceCollection.AddScoped<DeleteUnusedDatabasesTask>();

            Provider = serviceCollection.BuildServiceProvider();
        }
    }
}
