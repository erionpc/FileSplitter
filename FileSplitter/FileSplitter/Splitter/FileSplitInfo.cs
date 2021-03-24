using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter.Splitter
{
    public class FileSplitInfo
    {
        public FileSplitInfo(string filePath, int numberOfChunks)
        {
            this.FilePath = filePath;
            this.NumberOfChunks = numberOfChunks;
        }

        public FileSplitInfo(string filePath, long chunkSize)
        {
            this.FilePath = filePath;
            this.ChunkSize = chunkSize;
        }

        public string FilePath { get; set; }
        public int NumberOfChunks { get; set; }
        public long ChunkSize { get; set; }

        public override string ToString() =>
            $"{nameof(FileSplitInfo)}:{Environment.NewLine}" + 
            $"{nameof(FilePath)} = {FilePath}{Environment.NewLine}" +
            (NumberOfChunks > 0 ? $"{nameof(NumberOfChunks)} = {NumberOfChunks}{Environment.NewLine}" : "") +
            (ChunkSize > 0 ? $"{nameof(ChunkSize)} = {ChunkSize}" : "");
    }
}
