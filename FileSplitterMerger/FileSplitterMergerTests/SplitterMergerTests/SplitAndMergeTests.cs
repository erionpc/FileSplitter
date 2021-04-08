using FileSplitterMerger;
using FileSplitterMerger.ArgParser;
using FileSplitterMerger.Enums;
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
    public class SplitAndMergeTests : SplitterMergerTestsBase
    {
        static readonly ArgumentInfo _splitArgument = OperationOptionsEnum.Split.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _mergeArgument = OperationOptionsEnum.Merge.GetAttribute<ArgumentInfo>();

        static readonly ArgumentInfo _split_filePathArgument = SplitOptionsEnum.FilePath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _split_destinationPathArgument = SplitOptionsEnum.DestinationPath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _split_numberOfChunksArgument = SplitOptionsEnum.NumberOfChunks.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _split_chunkSizeArgument = SplitOptionsEnum.ChunkSize.GetAttribute<ArgumentInfo>();

        static readonly ArgumentInfo _merge_destinationFileArgument = MergeOptionsEnum.DestinationFilePath.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _merge_filePartsArgument = MergeOptionsEnum.FileParts.GetAttribute<ArgumentInfo>();

        public SplitAndMergeTests() : base()
        {
            
        }

        public static IEnumerable<object[]> SplitAndMergeTestData()
        {
            File.Copy($@"{AssemblyDirectory}\TestFiles\testA.txt", $@"{AssemblyDirectory}\TestFiles\SplitAndMergeTest_testFile_testA.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testB.txt", $@"{AssemblyDirectory}\TestFiles\SplitAndMergeTest_testFile_testB.txt", true);
            File.Copy($@"{AssemblyDirectory}\TestFiles\testC.txt", $@"{AssemblyDirectory}\TestFiles\SplitAndMergeTest_testFile_testC.txt", true);

            FileInfo fileInfoA = new FileInfo($@"{AssemblyDirectory}\TestFiles\SplitAndMergeTest_testFile_testA.txt");
            FileInfo fileInfoB = new FileInfo($@"{AssemblyDirectory}\TestFiles\SplitAndMergeTest_testFile_testB.txt");
            FileInfo fileInfoC = new FileInfo($@"{AssemblyDirectory}\TestFiles\SplitAndMergeTest_testFile_testC.txt");

            string splitDestinationPath = $@"{AssemblyDirectory}\TestFiles";
            string mergeDestinationPath = $@"{AssemblyDirectory}\TestFiles\SplitAndMergeTest.txt";

            yield return new object[]
            {
                "test correct split and merge with number of chunks",
                new string[] 
                { 
                    _splitArgument.ArgumentSwitch, 
                    _split_filePathArgument.ArgumentSwitch, 
                    fileInfoA.FullName, 
                    _split_numberOfChunksArgument.ArgumentSwitch, 
                    "3", 
                    _split_destinationPathArgument.ArgumentSwitch,
                    splitDestinationPath
                },
                new string[] 
                { 
                    _mergeArgument.ArgumentSwitch, 
                    _merge_destinationFileArgument.ArgumentSwitch, 
                    mergeDestinationPath, 
                    _merge_filePartsArgument.ArgumentSwitch,
                    ProcessUtils.GetChunkFileName(fileInfoA.Name, fileInfoA.DirectoryName, 1),
                    ProcessUtils.GetChunkFileName(fileInfoA.Name, fileInfoA.DirectoryName, 2),
                    ProcessUtils.GetChunkFileName(fileInfoA.Name, fileInfoA.DirectoryName, 3) 
                },
                fileInfoA.FullName,
                mergeDestinationPath,
                null
            };

            yield return new object[]
            {
                "test correct split and merge with size of chunks",
                new string[]
                {
                    _splitArgument.ArgumentSwitch,
                    _split_filePathArgument.ArgumentSwitch,
                    fileInfoA.FullName,
                    _split_chunkSizeArgument.ArgumentSwitch,
                    "50000",
                    _split_destinationPathArgument.ArgumentSwitch,
                    splitDestinationPath
                },
                new string[]
                {
                    _mergeArgument.ArgumentSwitch,
                    _merge_destinationFileArgument.ArgumentSwitch,
                    mergeDestinationPath,
                    _merge_filePartsArgument.ArgumentSwitch,
                    ProcessUtils.GetChunkFileName(fileInfoA.Name, fileInfoA.DirectoryName, 1),
                    ProcessUtils.GetChunkFileName(fileInfoA.Name, fileInfoA.DirectoryName, 2),
                    ProcessUtils.GetChunkFileName(fileInfoA.Name, fileInfoA.DirectoryName, 3)
                },
                fileInfoA.FullName,
                mergeDestinationPath,
                null
            };
        }

        [Theory]
        [MemberData(nameof(SplitAndMergeTestData))]
        public async Task SplitAndMergeJointTest(string testCase,
                                                 string[] splitArgs,
                                                 string[] mergeArgs,
                                                 string originalFile,
                                                 string splitAndMergedFile,
                                                 Exception expectedException)
        {
            try
            {
                var app = new App(Configuration, new ArgumentParser());

                await app.Run(splitArgs);

                await app.Run(mergeArgs);

                Assert.NotNull(testCase);
                Assert.True(File.Exists(splitAndMergedFile));

                int bufferSize = Configuration.GetValue<int>("SplitterConfig:Buffersize");

                FileInfo originalFileInfo = new FileInfo(originalFile);
                FileInfo splitAndMergedFileInfo = new FileInfo(splitAndMergedFile);
                Assert.Equal(originalFileInfo.Length, splitAndMergedFileInfo.Length);

                using (var originalStream = new FileStream(originalFileInfo.FullName,
                                                           FileMode.Open,
                                                           FileAccess.Read,
                                                           FileShare.Read,
                                                           bufferSize,
                                                           FileOptions.Asynchronous))
                {
                    using (var actualStream = new FileStream(splitAndMergedFileInfo.FullName,
                                                             FileMode.Open,
                                                             FileAccess.Read,
                                                             FileShare.Read,
                                                             bufferSize,
                                                             FileOptions.Asynchronous))
                    {
                        if (!originalStream.CanRead)
                            throw new FileSplitterMergerException($"Can't read file: '{originalFileInfo.FullName}'");

                        if (!actualStream.CanRead)
                            throw new FileSplitterMergerException($"Can't read file: '{splitAndMergedFileInfo.FullName}'");

                        long bytesRead = 0;
                        while (bytesRead < originalFileInfo.Length)
                        {
                            int currentBufferSize = ProcessUtils.GetCurrentBufferSize(bytesRead, originalFileInfo.Length, bufferSize);
                            byte[] originalBuffer = new byte[currentBufferSize];
                            byte[] actualBuffer = new byte[currentBufferSize];

                            await originalStream.ReadAsync(originalBuffer, 0, currentBufferSize);
                            await actualStream.ReadAsync(actualBuffer, 0, currentBufferSize);

                            for (int i = 0; i < actualBuffer.Length; i++)
                            {
                                Assert.Equal(originalBuffer[i], actualBuffer[i]);
                            }

                            bytesRead += currentBufferSize;
                        }
                    }
                }
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
                DirectoryInfo dirInfo = new FileInfo(splitAndMergedFile).Directory;
                foreach (var file in dirInfo.GetFiles("SplitAndMergeTest_testFile_*.part_*"))
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
