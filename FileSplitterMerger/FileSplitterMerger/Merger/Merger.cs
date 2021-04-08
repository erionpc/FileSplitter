using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger.Merger
{
    public class Merger : IMerger
    {
        protected IConfiguration Configuration { get; }

        public Merger(IConfiguration config)
        {
            this.Configuration = config;
        }

        private int _bufferSize;

        internal int BufferSize
        {
            get
            {
                if (_bufferSize <= 0)
                {
                    int defaultSize = Configuration.GetValue<int>("SplitterConfig:Buffersize");
                    
                    using (Process proc = Process.GetCurrentProcess())
                    {
                        if (FileMergingInfo.FileParts?.Any() ?? false)
                        {
                            var smallestChunkSize = FileMergingInfo.FileParts.Select(x => new FileInfo(x).Length).OrderBy(x => x).First();
                            long memory = proc.PrivateMemorySize64;
                            _bufferSize = ProcessUtils.GetMemoryOptimisedBufferSize(memory, smallestChunkSize);
                        }
                        else
                            _bufferSize = defaultSize;
                    }
                }
                return _bufferSize;
            }
            set { _bufferSize = value; }
        }

        public FileMergeInfo FileMergingInfo { get; set; }

        public async Task MergeFiles()
        {
            if (!FileMergingInfo.FileParts?.Any() ?? true)
                throw new FileSplitterMergerException("No file parts provided");

            var filePartsInfo = FileMergingInfo.FileParts.Select(x => new FileInfo(x)).ToList();

            if (filePartsInfo.Any(x => !x.Exists))
                throw new FileSplitterMergerException($"The following files don't exist: {string.Join(", ", filePartsInfo.Where(x => !x.Exists))}");

            long totalChunksSize = filePartsInfo.Sum(x => x.Length);
            var destinationFileInfo = new FileInfo(FileMergingInfo.DestinationFile);
            long destinationFileSize = 0;

            if (File.Exists(FileMergingInfo.DestinationFile))
                File.Delete(FileMergingInfo.DestinationFile);

            if (FileMergingInfo.FileParts.Count() == 1)
            {
                File.Copy(FileMergingInfo.FileParts.First(), FileMergingInfo.DestinationFile);
                return;
            }

            using (var writeStream = new FileStream(FileMergingInfo.DestinationFile,
                                                    FileMode.Append,
                                                    FileAccess.Write,
                                                    FileShare.Read,
                                                    BufferSize,
                                                    FileOptions.Asynchronous))
            {
                if (!writeStream.CanWrite)
                    throw new FileSplitterMergerException($"Can't write to path: '{FileMergingInfo.DestinationFile}'");

                for (int i = 0; i < FileMergingInfo.FileParts.Count(); i++)
                {
                    string currentChunk = FileMergingInfo.FileParts.ElementAt(i);
                    FileInfo currentChunkFileInfo = new FileInfo(currentChunk);
                    long currentChunkSize = currentChunkFileInfo.Length;

                    using var readStream = new FileStream(FileMergingInfo.FileParts.ElementAt(i),
                                                          FileMode.Open,
                                                          FileAccess.Read,
                                                          FileShare.Read,
                                                          BufferSize,
                                                          FileOptions.Asynchronous);

                    if (!readStream.CanRead)
                        throw new FileSplitterMergerException($"Can't read file: '{currentChunk}'");

                    long bytesCopiedFromChunk = 0;
                    while (bytesCopiedFromChunk < currentChunkSize)
                    {
                        int currentBufferSize = ProcessUtils.GetCurrentBufferSize(bytesCopiedFromChunk, currentChunkSize, BufferSize);

                        byte[] currentBuffer = new byte[currentBufferSize];
                        await readStream.ReadAsync(currentBuffer, 0, currentBufferSize);
                        await writeStream.WriteAsync(currentBuffer, 0, currentBufferSize);

                        bytesCopiedFromChunk += currentBufferSize;

                        if (bytesCopiedFromChunk == currentChunkSize)
                        {
                            await writeStream.FlushAsync();
                            destinationFileSize += currentChunkSize;
                        }
                    }
                }
            }

            if (destinationFileSize - totalChunksSize != 0)
                throw new FileSplitterMergerException($"File not split correctly! Difference in bytes: { totalChunksSize - destinationFileSize }");
        }
    }
}
