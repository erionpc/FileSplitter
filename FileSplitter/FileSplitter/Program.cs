using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileSplitter.Splitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileSplitter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var serviceProvider = Startup.GetServiceProvider();
                await serviceProvider.GetService<App>().Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Environment.NewLine}{ex}");
            }
        }
    }
}
