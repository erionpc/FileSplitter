using Xunit;
using FileSplitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSplitter.Splitter;

namespace FileSplitter.Tests
{
    public class ArgumentParserTests
    {
        static readonly ArgumentInfo _filePathArgument = SwitchEnum.FilePath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _numberOfChunksArgument = SwitchEnum.NumberOfChunks.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _chunkSizeArgument = SwitchEnum.ChunkSize.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _infoArgument = SwitchEnum.Info.GetAttribute<ArgumentInfo>();

        public static IEnumerable<object[]> TestData() 
        {
            string filePath = @"C:\filepath\file.txt";
            yield return new object[] 
            { 
                "test with /f and /c", 
                new string[] { "/f", filePath, "/c", "3" },
                new FileSplitInfo(filePath, 3),
                null
            };
            yield return new object[]
            {
                "test with /f and /s",
                new string[] { "/f", filePath, "/s", "3" },
                new FileSplitInfo(filePath, (long)3),
                null
            };
            yield return new object[]
            {
                "test with only /f",
                new string[] { "/f", filePath },
                null,
                new FileSplitException($"Please specify either {_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with /f, /c and /s",
                new string[] { "/f", filePath, "/c", "3", "/s", "3"  },
                null,
                new FileSplitException($"Please specify either {_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with /f and /c 0",
                new string[] { "/f", filePath, "/c", "0" },
                null,
                new FileSplitException($"Please specify either {_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with only /c",
                new string[] { "/c", "3" },
                null,
                new FileSplitException($"{_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with only /s",
                new string[] { "/s", "3" },
                null,
                new FileSplitException($"{_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with /f and /a",
                new string[] { "/f", filePath, "/a", "3" },
                null,
                new FileSplitException($"Unrecognised switch: /a")
            };
            yield return new object[]
            {
                "test with /f and /c without filename",
                new string[] { "/f", "/c", "3" },
                null,
                new FileSplitException($"{_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with /f and /c without value",
                new string[] { "/f", filePath, "/c" },
                null,
                new FileSplitException($"No value supplied for /c")
            };
            yield return new object[]
            {
                "test with /f and 2 /c",
                new string[] { "/f", filePath, "/c", "3", "/c", "3" },
                null,
                new FileSplitException($"Duplicated switches")
            };
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void BuildFileSplitInfoTest(string testCase, string[] arguments, FileSplitInfo expectedResult, Exception expectedException)
        {
            try
            {
                var argumentParser = new ArgumentParser
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

        // Todo: Add unit tests for NumberOfChunkSplitter
        // Todo: Add unit tests for SizeOfChunkSplitter
        // Todo: Add unit tests for Merger
    }
}