using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger.Splitter
{
    public class FileSplitInfo
    {
        public FileSplitInfo(string filePath, int numberOfChunks, string destinationPath)
        {
            this.FilePath = filePath;
            this.NumberOfChunks = numberOfChunks;
            this.DestinationPath = destinationPath;
        }

        public FileSplitInfo(string filePath, string destinationPath, long chunkSize)
        {
            this.FilePath = filePath;
            this.ChunkSize = chunkSize;
            this.DestinationPath = destinationPath;
        }

        public string FilePath { get; set; }
        public int NumberOfChunks { get; set; }
        public long ChunkSize { get; set; }
        public string DestinationPath { get; set; }

        public override string ToString() =>
            $"{nameof(FileSplitInfo)}:{Environment.NewLine}" + 
            $"{nameof(FilePath)} = {FilePath}{Environment.NewLine}" +
            $"{nameof(DestinationPath)} = {DestinationPath}{Environment.NewLine}" +
            (NumberOfChunks > 0 ? $"{nameof(NumberOfChunks)} = {NumberOfChunks}{Environment.NewLine}" : "") +
            (ChunkSize > 0 ? $"{nameof(ChunkSize)} = {ChunkSize}" : "");
    }
}
