using FileSplitterMerger.ArgParser;
using FileSplitterMerger.Enums;
using FileSplitterMerger.Merger;
using FileSplitterMerger.Splitter;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger
{
    public class App
    {
        private readonly IConfiguration _config;
        private readonly IArgumentParser _argumentParser;

        private ISplitter _fileSplitter;
        private IMerger _fileMerger;

        public App(IConfiguration config, IArgumentParser argParser)
        {
            _config = config;
            _argumentParser = argParser;
        }

        public async Task Run(string[] args)
        {
            try
            {
                _argumentParser.Arguments = args;
                var operation = _argumentParser.GetOperation();

                switch(operation)
                {
                    case OperationOptionsEnum.Info:
                    default:
                        Console.WriteLine($"{Environment.NewLine}Supported syntax:{Environment.NewLine}{PrintOptions()}");
                        break;

                    case OperationOptionsEnum.Split:
                        await FileSplit();
                        break;

                    case OperationOptionsEnum.Merge:
                        await FileMerge();
                        break;
                }
            }
            catch (FileSplitterMergerException ex)
            {
                Console.WriteLine($"{Environment.NewLine}{ex.Message}{Environment.NewLine}");
                Console.WriteLine($"Supported syntax:{Environment.NewLine}{Environment.NewLine}{PrintOptions()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Environment.NewLine}{ex}");
            }
        }

        private async Task FileSplit()
        {
            var fileSplitInfo = _argumentParser.BuildFileSplitInfo();

            Console.WriteLine($"{fileSplitInfo}{Environment.NewLine}" +
                          $"Splitting file...");

            if (fileSplitInfo.NumberOfChunks > 0)
                _fileSplitter = new NumberOfChunksSplitter(_config);
            else if (fileSplitInfo.ChunkSize > 0)
                _fileSplitter = new SizeOfChunksSplitter(_config);

            _fileSplitter.FileSplittingInfo = fileSplitInfo;
            await _fileSplitter.Split();

            Console.WriteLine($"Created files:{Environment.NewLine}" +
                              $"{string.Join(Environment.NewLine, _fileSplitter.CreatedFiles)}");
        }

        private async Task FileMerge()
        {
            var fileMergeInfo = _argumentParser.BuildFileMergeInfo();

            Console.WriteLine($"{Environment.NewLine}{fileMergeInfo}{Environment.NewLine}{Environment.NewLine}" +
                          $"Merging files...{Environment.NewLine}");

            _fileMerger = new Merger.Merger(_config)
            {
                FileMergingInfo = fileMergeInfo
            };
            await _fileMerger.MergeFiles();

            Console.WriteLine($"Files merged into: {fileMergeInfo.DestinationFile}");
        }

        private static string PrintOptions()
        {
            string options = $"Operation options:";
            foreach (OperationOptionsEnum option in Enum.GetValues(typeof(OperationOptionsEnum)))
            {
                var argumentInfo = option.GetAttribute<ArgumentInfo>();
                options += $"{Environment.NewLine}{argumentInfo.ArgumentSwitch} = {argumentInfo.ArgumentDescription}";
            }

            options += $"{Environment.NewLine}{Environment.NewLine}Split options:";
            foreach (SplitOptionsEnum option in Enum.GetValues(typeof(SplitOptionsEnum)))
            {
                var argumentInfo = option.GetAttribute<ArgumentInfo>();
                options += $"{Environment.NewLine}{argumentInfo.ArgumentSwitch} = {argumentInfo.ArgumentDescription}";
            }

            options += $"{Environment.NewLine}{Environment.NewLine}Merge options: {Environment.NewLine}";
            foreach (MergeOptionsEnum option in Enum.GetValues(typeof(MergeOptionsEnum)))
            {
                var argumentInfo = option.GetAttribute<ArgumentInfo>();
                options += $"{Environment.NewLine}{argumentInfo.ArgumentSwitch} = {argumentInfo.ArgumentDescription}";
            }

            return options;
        }
    }
}
