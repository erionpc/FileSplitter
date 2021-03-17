using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSplitter.Splitter
{
    internal class SizeOfChunksSplitter : SplitterBase
    {
        public SizeOfChunksSplitter(FileSplitInfo fileSplitInfo) : base(fileSplitInfo)
        {
        }

        public override Task Split()
        {
            // Todo: Implement SizeOfChunksSplitter
            throw new NotImplementedException();
        }
    }
}