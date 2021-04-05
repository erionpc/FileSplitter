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
    public class SizeOfChunksSplitterTests : SplitterMergerTestsBase
    {
        public SizeOfChunksSplitterTests() : base()
        {
            
        }

        public static IEnumerable<object[]> SizeOfChunksSplitter_TestData()
        {
            File.Copy($@"{AssemblyDirectory}\TestFiles\testA.txt", $@"{AssemblyDirectory}\TestFiles\SizeOfChunksSplitterTests_testFile_testA.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testB.txt", $@"{AssemblyDirectory}\TestFiles\SizeOfChunksSplitterTests_testFile_testB.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testC.txt", $@"{AssemblyDirectory}\TestFiles\SizeOfChunksSplitterTests_testFile_testC.txt", true);

            FileInfo fileInfoA = new FileInfo($@"{AssemblyDirectory}\TestFiles\SizeOfChunksSplitterTests_testFile_testA.txt");
            FileInfo fileInfoB = new FileInfo($@"{AssemblyDirectory}\TestFiles\SizeOfChunksSplitterTests_testFile_testB.txt");
            FileInfo fileInfoC = new FileInfo($@"{AssemblyDirectory}\TestFiles\SizeOfChunksSplitterTests_testFile_testC.txt");

            string destinationPath = $@"{AssemblyDirectory}\TestFiles";

            yield return new object[]
            {
                "test correct split in 6 chunks",
                new FileSplitInfo(fileInfoA.FullName, destinationPath, (long)Math.Ceiling((double)fileInfoA.Length / 5) - 13),
                null
            };
            yield return new object[]
            {
                "test correct split in 13 chunks",
                new FileSplitInfo(fileInfoA.FullName, destinationPath, (long)Math.Ceiling((double)fileInfoA.Length / 14) + 29),
                null
            };
            yield return new object[]
            {
                "test correct split in 1 chunk",
                new FileSplitInfo(fileInfoB.FullName, destinationPath, fileInfoB.Length),
                null
            };
            yield return new object[]
            {
                "test correct split in 2 chunks",
                new FileSplitInfo(fileInfoB.FullName, destinationPath, (long)Math.Ceiling((double)fileInfoB.Length / 2) + 7),
                null
            };
            yield return new object[]
            {
                "test split with chunks bigger than file size",
                new FileSplitInfo(fileInfoC.FullName, destinationPath, (int)(fileInfoC.Length + 10)),
                new FileSplitterMergerException($"The file size is smaller than the requested chunk size: {fileInfoC.Length}B")
            };
        }

        [Theory]
        [MemberData(nameof(SizeOfChunksSplitter_TestData))]
        public async Task SizeOfChunksSplitTest(string testCase, FileSplitInfo fileSplittingInfo, Exception expectedException)
        {
            try
            {
                var splitter = new SizeOfChunksSplitter(Configuration)
                {
                    FileSplittingInfo = fileSplittingInfo
                };
                await splitter.Split();

                Assert.NotNull(testCase);
                
                FileInfo inputFile = new FileInfo(fileSplittingInfo.FilePath);
                long totalBytes = 0;
                int numberOfChunks = (int)Math.Ceiling((double)inputFile.Length / fileSplittingInfo.ChunkSize);
                if (numberOfChunks > 1)
                { 
                    for (int i = 0; i < numberOfChunks; i++)
                    {
                        string chunkFileName = $"{inputFile.DirectoryName}{Path.DirectorySeparatorChar}{inputFile.Name}.part_{i + 1}";
                        FileInfo chunkFileInfo = new FileInfo(chunkFileName);
                    
                        Assert.True(File.Exists(chunkFileName), $"{testCase} failed: file chunk doesn't exist '{chunkFileName}'");
                    
                        totalBytes += chunkFileInfo.Length;
                    }
                }
                else
                    totalBytes = inputFile.Length;

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
                DirectoryInfo dirInfoParts = new DirectoryInfo(fileSplittingInfo.DestinationPath);
                foreach (var file in dirInfoParts.GetFiles("SizeOfChunksSplitterTests_testFile_*.part_*"))
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
