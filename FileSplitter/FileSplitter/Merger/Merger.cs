using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter.Merger
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

        public void MergeFiles(IEnumerable<string> fileChunks, string destinationFile)
        {
            if (!fileChunks?.Any() ?? true)
                throw new FileSplitException("No file parts provided");

            var destinationFileInfo = new FileInfo(destinationFile);

            long destinationFileSize = 0;

            using (var writeStream = new FileStream(destinationFile,
                                                    FileMode.Create,
                                                    FileAccess.Write,
                                                    FileShare.Read,
                                                    BufferSize,
                                                    FileOptions.Asynchronous))
            {
                if (!writeStream.CanWrite)
                    throw new FileSplitException($"Can't write to path: '{destinationFile}'");

                for (int i = 0; i < fileChunks.Count(); i++)
                {
                    using (var readStream = new FileStream(fileInfo.FullName,
                                                   FileMode.Open,
                                                   FileAccess.Read,
                                                   FileShare.Read,
                                                   BufferSize,
                                                   FileOptions.Asynchronous))
                {
                
                    string chunkFileName = GetChunkFileName(fileInfo, i + 1);
                    
                        if (!readStream.CanRead)
                            throw new FileSplitException($"Can't read file: '{FileSplittingInfo.FilePath}'");

                        
                        long currentChunkSize = 0;
                        while (currentChunkSize < chunkSize)
                        {
                            int currentBufferSize = GetCurrentBufferSize(currentChunkSize, chunkSize);

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

            // Todo: Implement merger
        }
    }
}
