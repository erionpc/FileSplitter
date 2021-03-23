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

        public override Task Split()
        {
            // Todo: Implement SizeOfChunksSplitter
            throw new NotImplementedException();
        }
    }
}