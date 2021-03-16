using System;
using System.Collections.Generic;
using System.IO;

namespace FileSplitter
{
    internal class Splitter
    {
        private FileSplitInfo _fileSplitInfo;
        private int _BufferSize { 
            get 
            {
                int defaultSize = 4096;
                if (_fileSplitInfo.ChunkSize > 0 && _fileSplitInfo.ChunkSize < defaultSize)
                    return (int)_fileSplitInfo.ChunkSize;
                else
                    return defaultSize;
            }
        } 

        public Splitter(FileSplitInfo fileSplitInfo)
        {
            _fileSplitInfo = fileSplitInfo;
        }

        public IEnumerable<string> CreatedFiles { get; private set; }

        internal void Split()
        {
            if (_fileSplitInfo.NumberOfChunks > 0)
                SplitInNumberOfChunks();
            else if (_fileSplitInfo.ChunkSize > 0)
                SplitInChunksOfSize();
        }

        private void SplitInNumberOfChunks()
        {
            var fileInfo = new FileInfo(_fileSplitInfo.FilePath);
            long chunkSize = (long)Math.Ceiling((double)(fileInfo.Length / 3));
            var dirInfo = new DirectoryInfo(fileInfo.DirectoryName);

            int chunk = 1;
            using (var readStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, _BufferSize, FileOptions.None))
            {
                using (var writeStream = new FileStream($"{fileInfo.DirectoryName}{Path.DirectorySeparatorChar}{fileInfo.Name.Replace(fileInfo.Extension, "")}_{chunk}", 
                    FileMode.Open, FileAccess.Read, FileShare.Read, _BufferSize, FileOptions.None))
                {
                    readStream.CopyToAsync(writeStream, _BufferSize);
                }
            }
        }

        private void SplitInChunksOfSize()
        {
            throw new NotImplementedException();
        }
    }
}