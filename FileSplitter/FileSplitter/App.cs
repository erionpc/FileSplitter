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
        private readonly ISplitter _numberOfChunksSplitter;
        private readonly ISplitter _sizeOfChunksSplitter;
        private readonly IArgumentParser _argumentParser;

        public App(IConfiguration config, IArgumentParser argParser, Func<SplitterType, ISplitter> splitterTypeResolver)
        {
            _config = config;
            _numberOfChunksSplitter = splitterTypeResolver(SplitterType.NumberOfChunksSplitter);
            _sizeOfChunksSplitter = splitterTypeResolver(SplitterType.SizeOfChunksSplitter);
            _argumentParser = argParser;
        }

        public async Task Run(string[] args)
        {
            try
            {
                _argumentParser.Arguments = args;
                var fileSplitInfo = _argumentParser.BuildFileSplitInfo();

                if (_argumentParser.InfoRequestReceived())
                {
                    Console.WriteLine($"{Environment.NewLine}Supported syntax:{Environment.NewLine}{PrintOptions()}");
                }
                else
                {
                    Console.WriteLine($"{fileSplitInfo}{Environment.NewLine}" +
                                      $"Splitting file...");

                    ISplitter splitter = null;
                    if (fileSplitInfo.NumberOfChunks > 0)
                        splitter = _numberOfChunksSplitter;
                    else if (fileSplitInfo.ChunkSize > 0)
                        splitter = _sizeOfChunksSplitter;

                    splitter.FileSplittingInfo = fileSplitInfo;
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
