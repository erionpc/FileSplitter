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
                var argParser = new ArgumentParser(args);
                var fileSplitInfo = argParser.BuildFileSplitInfo();

                if (argParser.InfoRequestReceived())
                {
                    Console.WriteLine($"{Environment.NewLine}Supported syntax:{Environment.NewLine}{PrintOptions()}");
                }
                else
                {
                    Console.WriteLine($"{fileSplitInfo}{Environment.NewLine}" +
                                      $"Splitting file...");

                    SplitterBase splitter = fileSplitInfo.NumberOfChunks > 0 ? new NumberOfChunksSplitter(fileSplitInfo) : new SizeOfChunksSplitter(fileSplitInfo);
                    await splitter.Split();

                    Console.WriteLine($"Created files:{Environment.NewLine}" +
                                      $"{string.Join(Environment.NewLine, splitter.CreatedFiles)}");
                }
            }
            catch (FileSplitException ex)
            {
                Console.WriteLine($"{Environment.NewLine}{ex.Message}{Environment.NewLine}");
                Console.WriteLine($"Supported syntax:{PrintOptions()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Environment.NewLine}{ex}");
            }
        }

        private static string PrintOptions()
        {
            string options = "";
            foreach (SwitchEnum switchOption in Enum.GetValues(typeof(SwitchEnum)))
            {
                var argumentInfo = switchOption.GetAttribute<ArgumentInfo>();
                options += $"{Environment.NewLine}{argumentInfo.ArgumentSwitch} = {argumentInfo.ArgumentDescription}";
            }

            return options;
        }
    }
}
