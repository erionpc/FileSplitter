using FileSplitterMerger;
using FileSplitterMerger.ArgParser;
using FileSplitterMerger.Splitter;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileSplitterMergerTests.SplitterMergerTests
{
    public class NumberOfChunksSplitterTests : SplitterMergerTestsBase
    {
        public NumberOfChunksSplitterTests() : base()
        {
            
        }

        public static IEnumerable<object[]> NumberOfChunksSplitter_TestData()
        {
            File.Copy($@"{AssemblyDirectory}\TestFiles\testA.txt", $@"{AssemblyDirectory}\TestFiles\NumberOfChunksSplitterTests_testFile_testA.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testB.txt", $@"{AssemblyDirectory}\TestFiles\NumberOfChunksSplitterTests_testFile_testB.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testC.txt", $@"{AssemblyDirectory}\TestFiles\NumberOfChunksSplitterTests_testFile_testC.txt", true);

            FileInfo fileInfoA = new FileInfo($@"{AssemblyDirectory}\TestFiles\NumberOfChunksSplitterTests_testFile_testA.txt");
            FileInfo fileInfoB = new FileInfo($@"{AssemblyDirectory}\TestFiles\NumberOfChunksSplitterTests_testFile_testB.txt");
            FileInfo fileInfoC = new FileInfo($@"{AssemblyDirectory}\TestFiles\NumberOfChunksSplitterTests_testFile_testC.txt");

            string destinationPath = $@"{AssemblyDirectory}\TestFiles";

            yield return new object[]
            {
                "test correct split with 3 chunks",
                new FileSplitInfo(fileInfoA.FullName, 3, destinationPath),
                null
            };
            yield return new object[]
            {
                "test correct split with 1 chunk",
                new FileSplitInfo(fileInfoA.FullName, 1, destinationPath),
                null
            };
            yield return new object[]
            {
                "test correct split with 7 chunks",
                new FileSplitInfo(fileInfoB.FullName, 7, destinationPath),
                null
            };
            yield return new object[]
            {
                "test correct split with 13 chunks",
                new FileSplitInfo(fileInfoB.FullName, 13, destinationPath),
                null
            };
            yield return new object[]
            {
                "test correct split with 19 chunks",
                new FileSplitInfo(fileInfoB.FullName, 19, destinationPath),
                null
            };
            yield return new object[]
            {
                "test split with 0 chunks",
                new FileSplitInfo(fileInfoA.FullName, 0, destinationPath),
                new FileSplitterMergerException($"File not split correctly! Difference in bytes: { fileInfoA.Length }")
            };
            yield return new object[]
            {
                "test split with more chunks than file bytes",
                new FileSplitInfo(fileInfoC.FullName, (int)(fileInfoC.Length + 2), destinationPath),
                new FileSplitterMergerException($"File not split correctly! Difference in bytes: -2")
            };
        }

        [Theory]
        [MemberData(nameof(NumberOfChunksSplitter_TestData))]
        public async Task NumberOfChunksSplitTest(string testCase, FileSplitInfo fileSplittingInfo, Exception expectedException)
        {
            try
            {
                var splitter = new NumberOfChunksSplitter(Configuration)
                {
                    FileSplittingInfo = fileSplittingInfo
                };
                await splitter.Split();

                Assert.NotNull(testCase);
                
                FileInfo inputFile = new FileInfo(fileSplittingInfo.FilePath);
                long totalBytes = 0;

                for (int i = 0; i < fileSplittingInfo.NumberOfChunks; i++)
                {
                    string chunkFileName = $"{inputFile.DirectoryName}{Path.DirectorySeparatorChar}{inputFile.Name}.part_{i + 1}";
                    FileInfo chunkFileInfo = new FileInfo(chunkFileName);
                    
                    Assert.True(File.Exists(chunkFileName), $"{testCase} failed: file chunk doesn't exist '{chunkFileName}'");
                    
                    totalBytes += chunkFileInfo.Length;
                }

                Assert.Equal(inputFile.Length, totalBytes);
            }
            catch (Exception ex)
            {
                if (expectedException == null)
                    throw;

                Assert.Equal(expectedException.GetType().FullName, ex.GetType().FullName);
                Assert.Equal(expectedException.Message, ex.Message);
            }
            finally
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fileSplittingInfo.DestinationPath);
                foreach (var file in dirInfo.GetFiles("NumberOfChunksSplitterTests_testFile_*.part_*"))
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
