using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using FileSplitterMerger.Enums;
using FileSplitterMerger.Merger;
using FileSplitterMerger.ArgParser;

namespace FileSplitterMerger.Tests.ArgumentParserTests
{
    public class BuildFileMergeInfoTests
    {
        static readonly ArgumentInfo _mergeOperation = OperationOptionsEnum.Merge.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _merge_destinationFileArgument = MergeOptionsEnum.DestinationFilePath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _merge_filePartsArgument = MergeOptionsEnum.FileParts.GetAttribute<ArgumentInfo>();

        public static IEnumerable<object[]> BuildFileMergeInfo_TestData()
        {
            string filePath = @"C:\filepath\file.txt";
            string filePart1 = @"C:\filepath\file.txt.part1";
            string filePart2 = @"C:\filepath\file.txt.part2";
            string filePart3 = @"C:\filepath\file.txt.part3";
            string filePart4WithSpaces = @"C:\file path\file name with spaces.txt.part4";

            yield return new object[]
            {
                "test with correct merge options: 3 file parts",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_destinationFileArgument.ArgumentSwitch, filePath, _merge_filePartsArgument.ArgumentSwitch, filePart1, filePart2, filePart3 },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart3 }, filePath),
                null
            };
            yield return new object[]
            {
                "test with correct merge options: 3 file parts in the middle of the arguments",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_filePartsArgument.ArgumentSwitch, filePart1, filePart2, filePart3, _merge_destinationFileArgument.ArgumentSwitch, filePath },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart3 }, filePath),
                null
            };
            yield return new object[]
            {
                "test with correct merge options and double quotes for one of the paths",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_filePartsArgument.ArgumentSwitch, filePart1, $"\"{filePart2}\"", filePart3, _merge_destinationFileArgument.ArgumentSwitch, filePath },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart3 }, filePath),
                null
            };
            yield return new object[]
            {
                "test with correct merge options and spaces in path in last file part",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_filePartsArgument.ArgumentSwitch, filePart1, $"\"{filePart2}\"", filePart3, filePart4WithSpaces, _merge_destinationFileArgument.ArgumentSwitch, filePath },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart3, filePart4WithSpaces }, filePath),
                null
            };
            yield return new object[]
            {
                "test with correct merge options and paths with spaces and double quotes in the middle of the arguments",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_filePartsArgument.ArgumentSwitch, filePart1, $"\"{filePart2}\"", filePart4WithSpaces, filePart3, _merge_destinationFileArgument.ArgumentSwitch, filePath },
                new FileMergeInfo(new[] { filePart1, filePart2, filePart4WithSpaces, filePart3 }, filePath),
                null
            };
            yield return new object[]
            {
                "test with missing file parts argument",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_destinationFileArgument.ArgumentSwitch, filePath },
                null,
                new FileSplitterMergerException($"{_merge_filePartsArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with only missing destinatino file argument",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_filePartsArgument.ArgumentSwitch, filePart1 },
                null,
                new FileSplitterMergerException($"{_merge_destinationFileArgument.ArgumentDescription} not specified")
            };
            yield return new object[]
            {
                "test with unknown merge option",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_destinationFileArgument.ArgumentSwitch, filePath, "-a", "3" },
                null,
                new FileSplitterMergerException($"Unrecognised merge option: -a")
            };
            yield return new object[]
            {
                "test with duplicated arguments",
                new string[] { _mergeOperation.ArgumentSwitch, _merge_destinationFileArgument.ArgumentSwitch, filePath, _merge_filePartsArgument.ArgumentSwitch, filePart1, _merge_filePartsArgument.ArgumentSwitch, filePart2 },
                null,
                new FileSplitterMergerException($"Duplicated options")
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