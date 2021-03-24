using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter.Merger
{
    public interface IMerger
    {
        void MergeFiles(IEnumerable<string> fileChunks, string destinationFile);
    }
}
