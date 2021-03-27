using Xunit;
using FileSplitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSplitter.Splitter;
using FileSplitter.Enums;
using FileSplitter.FileMerger;

namespace FileSplitter.Tests.ArgumentParserTests
{
    public class BuildFileSplitInfoTests
    {
        static readonly ArgumentInfo _split_filePathArgument = SplitOptionsEnum.FilePath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _split_numberOfChunksArgument = SplitOptionsEnum.NumberOfChunks.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _split_chunkSizeArgument = SplitOptionsEnum.ChunkSize.GetAttribute<ArgumentInfo>();

        public static IEnumerable<object[]> BuildFileSplitInfo_TestData() 
        {
            string filePath = @"C:\filepath\file.txt";
            yield return new object[] 
            { 
                "test with /s /f and /n", 
                new string[] { "/s", "/f", filePath, "/n", "3" },
                new FileSplitInfo(filePath, 3),
                null
            };
            yield return new object[]
            {
                "test with /s, /f and /s",
                new string[] { "/s", "/f", filePath, "/s", "3" },
                new FileSplitInfo(filePath, (long)3),
                null
            };
            yield return new object[]
            {
                "test with only /s and /f",
                new string[] { "/s", "/f", filePath },
                null,
                new FileSplitException($"Please specify either {_split_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_split_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with /s, /f, /n and /s",
                new string[] { "/s", "/f", filePath, "/n", "3", "/s", "3"  },
                null,
                new FileSplitException($"Please specify either {_split_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_split_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with /s, /f and /n 0",
                new string[] { "/s", "/f", filePath, "/n", "0" },
                null,
                new FileSplitException($"Please specify either {_split_numberOfChunksArgument.ArgumentDescription.ToLower()} or {_split_chunkSizeArgument.ArgumentDescription.ToLower()}")
            };
            yield return new object[]
            {
                "test with only /s and /n",
                new string[] { "/s", "/n", "3" },
                null,
                new FileSplitException($"{_split_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with only /s and /s",
                new string[] { "/s", "/s", "3" },
                null,
                new FileSplitException($"{_split_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with /s, /f and /a",
                new string[] { "/s", "/f", filePath, "/a", "3" },
                null,
                new FileSplitException($"Unrecognised split option: /a")
            };
            yield return new object[]
            {
                "test with /s, /f and /n without filename",
                new string[] { "/s", "/f", "/n", "3" },
                null,
                new FileSplitException($"{_split_filePathArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with /s, /f and /n without value",
                new string[] { "/s", "/f", filePath, "/n" },
                null,
                new FileSplitException($"No value supplied for /n")
            };
            yield return new object[]
            {
                "test with /s, /f and 2 /n",
                new string[] { "/s", "/f", filePath, "/n", "3", "/n", "3" },
                null,
                new FileSplitException($"Duplicated options")
            };
        }

        [Theory]
        [MemberData(nameof(BuildFileSplitInfo_TestData))]
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
    }
}