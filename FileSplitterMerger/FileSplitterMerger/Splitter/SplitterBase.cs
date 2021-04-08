using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FileSplitterMerger.Splitter
{
    public abstract class SplitterBase : ISplitter
    {
        protected SplitterType _typeOfSplitter;

        public FileSplitInfo FileSplittingInfo { get; set; }
        
        protected IConfiguration Configuration { get; }

        public List<string> CreatedFiles { get; protected internal set; } = new List<string>();

        private int _bufferSize;

        internal int BufferSize
        {
            get
            {
                if (_bufferSize <= 0)
                { 
                    int defaultSize = Configuration.GetValue<int>("SplitterConfig:Buffersize");
                    if (FileSplittingInfo.ChunkSize > 0 && FileSplittingInfo.ChunkSize < defaultSize)
                        _bufferSize = (int)FileSplittingInfo.ChunkSize;
                    else
                    {
                        using (Process proc = Process.GetCurrentProcess())
                        {
                            long memory = proc.PrivateMemorySize64;
                            if (FileSplittingInfo.ChunkSize > 0)
                            {
                                _bufferSize = ProcessUtils.GetMemoryOptimisedBufferSize(memory, FileSplittingInfo.ChunkSize);
                            }
                            else if (FileSplittingInfo.NumberOfChunks > 0)
                            {
                                long fileSize = new FileInfo(FileSplittingInfo.FilePath).Length;
                                long chunkSize = (long)Math.Ceiling((double)fileSize / FileSplittingInfo.NumberOfChunks);
                                _bufferSize = ProcessUtils.GetMemoryOptimisedBufferSize(memory, chunkSize);
                            }
                            else
                                _bufferSize = defaultSize;
                        }
                    }   
                }
                return _bufferSize;
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
    }
}