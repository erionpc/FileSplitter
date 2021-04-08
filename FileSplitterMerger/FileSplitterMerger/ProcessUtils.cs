using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger
{
    public static class ProcessUtils
    {
        public static int GetMemoryOptimisedBufferSize(long memory, long chunkSize)
        {
            if (memory >= chunkSize)
                return chunkSize <= int.MaxValue ? (int)chunkSize : int.MaxValue;
            else
                return memory <= int.MaxValue ? (int)memory : int.MaxValue;
        }

        public static int GetCurrentBufferSize(long currentChunkSize, long chunkSize, int bufferSize)
        {
            if (currentChunkSize + bufferSize <= chunkSize)
                return bufferSize;
            else
            {
                if (bufferSize > chunkSize)
                    return (int)chunkSize;
                else
                    return (int)(chunkSize - currentChunkSize);
            }
        }

        public static string GetChunkFileName(string fileName, string path, int chunkNumber) =>
            $"{path}{Path.DirectorySeparatorChar}{fileName}.part_{chunkNumber}";
    }
}
