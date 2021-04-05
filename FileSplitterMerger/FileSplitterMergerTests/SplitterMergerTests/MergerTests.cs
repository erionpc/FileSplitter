using FileSplitterMerger;
using FileSplitterMerger.ArgParser;
using FileSplitterMerger.Merger;
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
    public class MergerTests : SplitterMergerTestsBase
    {
        public MergerTests() : base()
        {
            
        }

        public static IEnumerable<object[]> Merger_TestData()
        {
            File.Copy($@"{AssemblyDirectory}\TestFiles\testA.txt", $@"{AssemblyDirectory}\TestFiles\MergerTests_testFile_testA.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testB.txt", $@"{AssemblyDirectory}\TestFiles\MergerTests_testFile_testB.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testC.txt", $@"{AssemblyDirectory}\TestFiles\MergerTests_testFile_testC.txt", true);

            FileInfo fileInfoA = new FileInfo($@"{AssemblyDirectory}\TestFiles\MergerTests_testFile_testA.txt");
            FileInfo fileInfoB = new FileInfo($@"{AssemblyDirectory}\TestFiles\MergerTests_testFile_testB.txt");
            FileInfo fileInfoC = new FileInfo($@"{AssemblyDirectory}\TestFiles\MergerTests_testFile_testC.txt");

            string destinationPath = $@"{AssemblyDirectory}\TestFiles\MergedFile.txt";

            yield return new object[]
            {
                "test correct merge with 3 parts",
                new FileMergeInfo(
                    new List<string>() {
                        fileInfoA.FullName, 
                        fileInfoB.FullName, 
                        fileInfoC.FullName 
                    }, destinationPath),
                null
            };

            yield return new object[]
            {
                "test correct merge with 2 parts",
                new FileMergeInfo(
                    new List<string>() {
                        fileInfoB.FullName,
                        fileInfoC.FullName
                    }, destinationPath),
                null
            };

            yield return new object[]
            {
                "test correct merge with 1 part",
                new FileMergeInfo(
                    new List<string>() {
                        fileInfoB.FullName
                    }, destinationPath),
                null
            };
        }

        [Theory]
        [MemberData(nameof(Merger_TestData))]
        public async Task MergerTest(string testCase, FileMergeInfo fileMergingInfo, Exception expectedException)
        {
            try
            {
                var merger = new Merger(Configuration)
                {
                    FileMergingInfo = fileMergingInfo
                };

                await merger.MergeFiles();

                Assert.NotNull(testCase);
                Assert.True(File.Exists(fileMergingInfo.DestinationFile));

                FileInfo destinationFile = new FileInfo(fileMergingInfo.DestinationFile);
                var filePartsSize = fileMergingInfo.FileParts.Sum(x => new FileInfo(x).Length);
                Assert.Equal(filePartsSize, destinationFile.Length);
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
                if (File.Exists(fileMergingInfo.DestinationFile))
                    File.Delete(fileMergingInfo.DestinationFile);                
            }
        }
    }
}
