using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FileSplitter.Splitter
{
    internal abstract class SplitterBase : ISplitter
    {
        protected SplitterType _typeOfSplitter;

        public FileSplitInfo FileSplittingInfo { get; set; }
        
        protected IConfiguration Configuration { get; }

        public List<string> CreatedFiles { get; protected internal set; } = new List<string>();

        internal int BufferSize
        {
            get
            {
                int defaultSize = Configuration.GetValue<int>("SplitterConfig:Buffersize");
                if (FileSplittingInfo.ChunkSize > 0 && FileSplittingInfo.ChunkSize < defaultSize)
                    return (int)FileSplittingInfo.ChunkSize;
                else
                    return defaultSize;
            }
        }

        SplitterType ISplitter.TypeOfSplitter {
            get => _typeOfSplitter;
            set => _typeOfSplitter = value;
        }

        public SplitterBase(IConfiguration config)
        {
            this.Configuration = config;
        }

        abstract public Task Split();

        protected internal static string GetChunkFileName(FileInfo fileInfo, int chunkNumber) =>
            $"{fileInfo.DirectoryName}{Path.DirectorySeparatorChar}{fileInfo.Name}.part_{chunkNumber}";
    }
}