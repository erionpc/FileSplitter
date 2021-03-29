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
            yield return new object[]
            {
                "test A 3 chunks",
                new FileSplitInfo($@"{AssemblyDirectory}\TestFiles\testA.txt", 3),
                null
            };
        }

        [Theory]
        [MemberData(nameof(NumberOfChunksSplitter_TestData))]
        public void NumberOfChunksSplitTest(string testCase, FileSplitInfo fileSplittingInfo, Exception expectedException)
        {
            try
            {
                var splitter = new NumberOfChunksSplitter(Configuration);
                splitter.FileSplittingInfo = fileSplittingInfo;

                var splitTask = splitter.Split();
                splitTask.Wait();

                Assert.NotNull(testCase);

                FileInfo inputFile = new FileInfo(fileSplittingInfo.FilePath);

                long totalBytes = 0;
                for (int i = 0; i < fileSplittingInfo.NumberOfChunks; i++)
                {
                    string chunkFileName = $"{inputFile.DirectoryName}{Path.DirectorySeparatorChar}{inputFile.Name}.part_{i + 1}";
                    FileInfo chunkFileInfo = new FileInfo(chunkFileName);
                    
                    Assert.True(File.Exists(chunkFileName), $"{testCase} failed: file chunk doesn't exist '{chunkFileName}'");
                    
                    totalBytes += chunkFileInfo.Length;

                    // cleanup
                    File.Delete(chunkFileName);
                }

                Assert.Equal(inputFile.Length, totalBytes);
            }
            catch (Exception ex)
            {
                Assert.Equal(expectedException.GetType().FullName, ex.GetType().FullName);
                Assert.Equal(expectedException.Message, ex.Message);
            }
        }
    }
}
