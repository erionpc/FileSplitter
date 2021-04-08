using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSplitterMerger.Splitter
{
    public class NumberOfChunksSplitter : SplitterBase
    {
        public NumberOfChunksSplitter(IConfiguration config) : base(config)
        {
            _typeOfSplitter = SplitterType.NumberOfChunksSplitter;
        }

        public override async Task Split()
        {
            var fileInfo = new FileInfo(FileSplittingInfo.FilePath);
            long originalSize = fileInfo.Length;

            long chunkSize = (long)Math.Ceiling((double)fileInfo.Length / FileSplittingInfo.NumberOfChunks);
            long totalChunksSize = 0;
            
            using (var readStream = new FileStream(fileInfo.FullName,
                                                   FileMode.Open,
                                                   FileAccess.Read,
                                                   FileShare.Read,
                                                   BufferSize,
                                                   FileOptions.Asynchronous))
            {
                for (int i = 0; i < FileSplittingInfo.NumberOfChunks; i++)
                {
                    string chunkFileName = ProcessUtils.GetChunkFileName(fileInfo.Name, FileSplittingInfo.DestinationPath, i + 1);
                    using (var writeStream = new FileStream(chunkFileName,
                                                            FileMode.Create,
                                                            FileAccess.Write,
                                                            FileShare.Read,
                                                            BufferSize,
                                                            FileOptions.Asynchronous))
                    {
                        if (!readStream.CanRead)
                            throw new FileSplitterMergerException($"Can't read file: '{FileSplittingInfo.FilePath}'");

                        if (!writeStream.CanWrite)
                            throw new FileSplitterMergerException($"Can't write to path: '{chunkFileName}'");

                        if (totalChunksSize == originalSize)
                        {
                            // create empty file when all the bytes of the original file have been written (should just be one in edge cases)
                            await writeStream.WriteAsync(new ReadOnlyMemory<byte>());
                            continue;
                        }

                        long currentChunkSize = 0;
                        while (currentChunkSize < chunkSize)
                        {
                            int currentBufferSize = ProcessUtils.GetCurrentBufferSize(currentChunkSize, chunkSize, BufferSize);

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
                throw new FileSplitterMergerException($"File not split correctly! Difference in bytes: { originalSize - totalChunksSize }");
        }
    }
}