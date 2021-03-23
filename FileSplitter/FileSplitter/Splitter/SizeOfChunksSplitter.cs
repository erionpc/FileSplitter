using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSplitter.Splitter
{
    internal class SizeOfChunksSplitter : SplitterBase
    {
        public SizeOfChunksSplitter(IConfiguration config) : base(config)
        {
            _typeOfSplitter = SplitterType.SizeOfChunksSplitter;
        }

        public override async Task Split()
        {
            var fileInfo = new FileInfo(FileSplittingInfo.FilePath);
            long originalSize = fileInfo.Length;
            
            if (FileSplittingInfo.ChunkSize > originalSize)
                throw new FileSplitException($"The file size is smaller than the requested chunk size: {originalSize} B");

            if (FileSplittingInfo.ChunkSize == originalSize)
                return;

            int numberOfChunks = (int)Math.Ceiling((double)fileInfo.Length / FileSplittingInfo.ChunkSize);
            long totalChunksSize = 0;

            using (var readStream = new FileStream(fileInfo.FullName,
                                                   FileMode.Open,
                                                   FileAccess.Read,
                                                   FileShare.Read,
                                                   BufferSize,
                                                   FileOptions.Asynchronous))
            {
                for (int i = 0; i < numberOfChunks; i++)
                {
                    string chunkFileName = GetChunkFileName(fileInfo, i + 1);
                    using (var writeStream = new FileStream(chunkFileName,
                                                            FileMode.Create,
                                                            FileAccess.Write,
                                                            FileShare.Read,
                                                            BufferSize,
                                                            FileOptions.Asynchronous))
                    {
                        if (!readStream.CanRead)
                            throw new FileSplitException($"Can't read file: '{FileSplittingInfo.FilePath}'");

                        if (!writeStream.CanWrite)
                            throw new FileSplitException($"Can't write to path: '{chunkFileName}'");

                        long currentChunkSize = 0;
                        while (currentChunkSize < chunkSize)
                        {
                            int currentBufferSize = 0;

                            byte[] currentBuffer = new byte[currentBufferSize];
                            await readStream.ReadAsync(currentBuffer, 0, currentBufferSize);
                            await writeStream.WriteAsync(currentBuffer, 0, currentBufferSize);

                            currentChunkSize += currentBufferSize;

                            if (currentChunkSize == chunkSize)
                            {
                                await writeStream.FlushAsync();
                                await writeStream.DisposeAsync();
                                totalChunksSize += chunkSize;
                                CreatedFiles.Add(chunkFileName);
                            }
                        }
                    }
                    if (originalSize - totalChunksSize > 0 && originalSize - totalChunksSize < chunkSize)
                        chunkSize = originalSize - totalChunksSize;
                }
            }

            if (originalSize - totalChunksSize != 0)
                throw new FileSplitException($"File not split correctly! Difference in bytes: { originalSize - totalChunksSize }");
        }
    }
}