using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FileSplitter.Splitter
{
    internal abstract class SplitterBase
    {
        internal FileSplitInfo FileSplittingInfo { get; private set; }

        public List<string> CreatedFiles { get; protected internal set; } = new List<string>();

        internal int BufferSize 
        { 
            get 
            {
                int defaultSize = 4096;
                if (FileSplittingInfo.ChunkSize > 0 && FileSplittingInfo.ChunkSize < defaultSize)
                    return (int)FileSplittingInfo.ChunkSize;
                else
                    return defaultSize;
            }
        }

        public SplitterBase(FileSplitInfo fileSplitInfo)
        {
            FileSplittingInfo = fileSplitInfo;
        }

        abstract public Task Split();

        protected internal string GetChunkFileName(FileInfo fileInfo, int chunkNumber) =>
            $"{fileInfo.DirectoryName}{Path.DirectorySeparatorChar}{fileInfo.Name}.part_{chunkNumber}";
    }
}