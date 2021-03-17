using Xunit;
using FileSplitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter.Tests
{
    public class ArgumentParserTests
    {
        ArgumentInfo filePathArgument = SwitchEnum.FilePath.GetAttribute<ArgumentInfo>();
        ArgumentInfo numberOfChunksArgument = SwitchEnum.NumberOfChunks.GetAttribute<ArgumentInfo>();
        ArgumentInfo chunkSizeArgument = SwitchEnum.ChunkSize.GetAttribute<ArgumentInfo>();
        ArgumentInfo infoArgument = SwitchEnum.Info.GetAttribute<ArgumentInfo>();

        [Fact()]
        public void BuildFileSplitInfoTest_ValidForNumberOfChunks()
        {
            string filePath = @"C:\TestPath\testFile.txt";
            int numberOfChunks = 3;

            ArgumentParser parser = new ArgumentParser(new[] { filePathArgument.ArgumentSwitch, 
                                                               filePath, 
                                                               numberOfChunksArgument.ArgumentSwitch, 
                                                               numberOfChunks.ToString() });

            var expected = new FileSplitInfo(filePath, numberOfChunks);
            var actual = parser.BuildFileSplitInfo();

            Assert.Equal(expected.FilePath, actual.FilePath);
            Assert.Equal(expected.NumberOfChunks, actual.NumberOfChunks);
            Assert.Equal(expected.ChunkSize, actual.ChunkSize);
        }

        [Fact()]
        public void BuildFileSplitInfoTest_ValidForChunkSize()
        {
            string filePath = @"C:\TestPath\testFile.txt";
            long chunkSize = 50892;

            ArgumentParser parser = new ArgumentParser(new[] { filePathArgument.ArgumentSwitch,
                                                               filePath,
                                                               chunkSizeArgument.ArgumentSwitch,
                                                               chunkSize.ToString() });

            var expected = new FileSplitInfo(filePath, chunkSize);
            var actual = parser.BuildFileSplitInfo();

            Assert.Equal(expected.FilePath, actual.FilePath);
            Assert.Equal(expected.NumberOfChunks, actual.NumberOfChunks);
            Assert.Equal(expected.ChunkSize, actual.ChunkSize);
        }
    }
}