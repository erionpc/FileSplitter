using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter
{
    public enum SwitchEnum
    {
        [ArgumentInfo("/f", "File path")]
        FilePath,
        [ArgumentInfo("/c", "Number of chunks")]
        NumberOfChunks,
        [ArgumentInfo("/s", "File size in bytes")]
        ChunkSize,
        [ArgumentInfo("/i", "Syntax info")]
        Info
    }
}
