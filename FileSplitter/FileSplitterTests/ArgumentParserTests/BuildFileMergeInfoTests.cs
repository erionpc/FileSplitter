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
    public class BuildFileMergeInfoTests
    {
        static readonly ArgumentInfo _merge_destinationFileArgument = MergeOptionsEnum.DestinationFilePath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _merge_filePartsArgument = MergeOptionsEnum.FileParts.GetAttribute<ArgumentInfo>();

        public static IEnumerable<object[]> BuildFileMergeInfo_TestData()
        {
            string filePath = @"C:\filepath\file.txt";
            string filePart1 = @"C:\filepath\file.txt.part1";
            string filePart2 = @"C:\filepath\file.txt.part2";
            string filePart3 = @"C:\filepath\file.txt.part3";

            yield return new object[]
            {
                "test with /m, /d and /p",
                new string[] { "/m", "/d", filePath, "/p", filePart1, filePart2, filePart3 },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart3 }, filePath),
                null
            };
            yield return new object[]
            {
                "test with /m, /p and /d",
                new string[] { "/m", "/p", filePart1, filePart2, filePart3, "/d", filePath },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart3 }, filePath),
                null
            };
            yield return new object[]
            {
                "test with /m, /p and /d with double quotes",
                new string[] { "/m", "/p", filePart1, $"\"{filePart2}\"", filePart3, "/d", filePath },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart3 }, filePath),
                null
            };
            yield return new object[]
            {
                "test with only /m and /d",
                new string[] { "/m", "/d", filePath },
                null,
                new FileSplitException($"{_merge_filePartsArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with only /m and /p",
                new string[] { "/m", "/p", filePart1 },
                null,
                new FileSplitException($"{_merge_destinationFileArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with /m, /d and /a",
                new string[] { "/m", "/d", filePath, "/a", "3" },
                null,
                new FileSplitException($"Unrecognised merge option: /a")
            };
            yield return new object[]
            {
                "test with /m, /d and 2 /p",
                new string[] { "/m", "/d", filePath, "/p", filePart1, "/p", filePart2 },
                null,
                new FileSplitException($"Duplicated options")
            };
        }

        [Theory]
        [MemberData(nameof(BuildFileMergeInfo_TestData))]
        public void BuildFileMergeInfoTest(string testCase, string[] arguments, FileMergeInfo expectedResult, Exception expectedException)
        {
            try
            {
                var argumentParser = new ArgumentParser
                {
                    Arguments = arguments
                };
                var actual = argumentParser.BuildFileMergeInfo();

                Assert.NotNull(testCase);
                
                Assert.Equal(expectedResult.DestinationFile, actual.DestinationFile);
                
                if (expectedResult.FileParts?.Any() ?? false)
                { 
                    for (int i = 0; i < expectedResult.FileParts.Count(); i++)
                    {
                        Assert.Equal(expectedResult.FileParts.ElementAt(i), actual.FileParts.ElementAt(i));
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Equal(expectedException.GetType().FullName, ex.GetType().FullName);
                Assert.Equal(expectedException.Message, ex.Message);
            }
        }
    }
}