using Xunit;
using FileSplitterMerger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSplitterMerger.Splitter;
using FileSplitterMerger.Enums;
using FileSplitterMerger.Merger;
using FileSplitterMerger.ArgParser;

namespace FileSplitterMerger.Tests.ArgumentParserTests
{
    public class BuildFileSplitInfoTests
    {
        static readonly ArgumentInfo _splitArgument = OperationOptionsEnum.Split.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _filePathArgument = SplitOptionsEnum.FilePath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _destinationPathArgument = SplitOptionsEnum.DestinationPath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _numberOfChunksArgument = SplitOptionsEnum.NumberOfChunks.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _chunkSizeArgument = SplitOptionsEnum.ChunkSize.GetAttribute<ArgumentInfo>();

        public static IEnumerable<object[]> BuildFileSplitInfo_TestData() 
        {
            string filePath = @"C:\filepath\file.txt";
            string destinationPath = @"C:\filepath";

            yield return new object[] 
            { 
                "test with correct number of chunks splitting options", 
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _numberOfChunksArgument.ArgumentSwitch, "3", _destinationPathArgument.ArgumentSwitch, destinationPath },
                new FileSplitInfo(filePath, 3, destinationPath),
                null
            };
            yield return new object[]
            {
                "test with correct size of chunks splitting options",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _destinationPathArgument.ArgumentSwitch, destinationPath, _chunkSizeArgument.ArgumentSwitch, "3" },
                new FileSplitInfo(filePath, destinationPath, 3),
                null
            };
            yield return new object[]
            {
                "test with no splitting type option",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"Please specify either {_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with both splitting type options",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _numberOfChunksArgument.ArgumentSwitch, "3", _chunkSizeArgument.ArgumentSwitch, "3", _destinationPathArgument.ArgumentSwitch, destinationPath  },
                null,
                new FileSplitterMergerException($"Please specify either {_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with number of chunks option and value zero",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _numberOfChunksArgument.ArgumentSwitch, "0", _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"Please specify either {_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test without file path argument and number of chunks option",
                new string[] { _splitArgument.ArgumentSwitch, _numberOfChunksArgument.ArgumentSwitch, "3", _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"{_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test without file path argument and size of chunks option",
                new string[] { _splitArgument.ArgumentSwitch, _chunkSizeArgument.ArgumentSwitch, "3", _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"{_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test without destination path argument",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _numberOfChunksArgument.ArgumentSwitch, "3" },
                null,
                new FileSplitterMergerException($"{_destinationPathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with unknown option",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, "-a", "3", _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"Unrecognised split option: -a")
            };
            yield return new object[]
            {
                "test without a file name value",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, _numberOfChunksArgument.ArgumentSwitch, "3", _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"{_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test without a destination path value",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _destinationPathArgument.ArgumentSwitch, _numberOfChunksArgument.ArgumentSwitch, "3" },
                null,
                new FileSplitterMergerException($"{_destinationPathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test without number of chunks value",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _numberOfChunksArgument.ArgumentSwitch, _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"Please specify either {_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with duplicated valid options",
                new string[] { _splitArgument.ArgumentSwitch, _filePathArgument.ArgumentSwitch, filePath, _numberOfChunksArgument.ArgumentSwitch, "3", _numberOfChunksArgument.ArgumentSwitch, "3", _destinationPathArgument.ArgumentSwitch, destinationPath },
                null,
                new FileSplitterMergerException($"Duplicated options")
            };
        }

        [Theory]
        [MemberData(nameof(BuildFileSplitInfo_TestData))]
        public void BuildFileSplitInfoTest(string testCase, string[] arguments, FileSplitInfo expectedResult, Exception expectedException)
        {
            try
            {
                var argumentParser = new ArgumentParser()
                {
                    Arguments = arguments
                };
                var actual = argumentParser.BuildFileSplitInfo();

                Assert.NotNull(testCase);
                Assert.Equal(expectedResult.FilePath, actual.FilePath);
                Assert.Equal(expectedResult.NumberOfChunks, actual.NumberOfChunks);
                Assert.Equal(expectedResult.ChunkSize, actual.ChunkSize);
            }
            catch (Exception ex)
            {
                Assert.Equal(expectedException.GetType().FullName, ex.GetType().FullName);
                Assert.Equal(expectedException.Message, ex.Message);
            }
        }
    }
}