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
    public class Startup
    {
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            services.AddTransient<IArgumentParser, ArgumentParser>();
            services.AddTransient<ISplitter, NumberOfChunksSplitter>();
            services.AddTransient<ISplitter, SizeOfChunksSplitter>();
            services.AddTransient<App>();

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
