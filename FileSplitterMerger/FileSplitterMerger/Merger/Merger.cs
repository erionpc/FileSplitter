using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
                if (_bufferSize == 0)
                    _bufferSize = Configuration.GetValue<int>("SplitterConfig:Buffersize");

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
                        int currentBufferSize = GetCurrentBufferSize(bytesCopiedFromChunk, currentChunkSize);

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

        protected internal int GetCurrentBufferSize(long currentChunkSize, long chunkSize)
        {
            if (currentChunkSize + BufferSize <= chunkSize)
                return BufferSize;
            else
            {
                if (BufferSize > chunkSize)
                    return (int)chunkSize;
                else
                    return (int)(chunkSize - currentChunkSize);
            }
        }
    }
}
