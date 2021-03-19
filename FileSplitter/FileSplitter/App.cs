using FileSplitter.Splitter;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter
{
    public class App
    {
        private readonly IConfiguration _config;
        private readonly ISplitter _fileSplitter;

        public App(IConfiguration config, ISplitter fileSplitter)
        {
            _config = config;
            _fileSplitter = fileSplitter;
        }

        public async Task Run(string[] args)
        {
            try
            {
                var argumentParser = new ArgumentParser(args);
                var fileSplitInfo = argumentParser.BuildFileSplitInfo();

                if (argumentParser.InfoRequestReceived())
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
