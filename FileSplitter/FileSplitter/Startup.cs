using FileSplitter.Splitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter
{
    public delegate ISplitter SplitterTypeResolver(SplitterType splitterType);

    public class Startup
    {
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            // required to run the application
            services.AddTransient<App>();
            services.AddTransient<IArgumentParser, ArgumentParser>();

            services.AddScoped<ISplitter, NumberOfChunksSplitter>();
            services.AddScoped<ISplitter, SizeOfChunksSplitter>();

            services.AddTransient<SplitterTypeResolver>(serviceProvider => serviceTypeName => 
            {
                switch (serviceTypeName)
                {
                    case SplitterType.NumberOfChunksSplitter:
                        return serviceProvider.GetService<NumberOfChunksSplitter>();
                    case SplitterType.SizeOfChunksSplitter:
                        return serviceProvider.GetService<SizeOfChunksSplitter>();
                    default:
                        return null;
                }
            });

            return services;
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        internal static ServiceProvider GetServiceProvider() =>
            ConfigureServices().BuildServiceProvider();
    }
}
