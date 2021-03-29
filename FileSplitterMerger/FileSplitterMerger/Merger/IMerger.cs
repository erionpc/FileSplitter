using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger.Merger
{
    public interface IMerger
    {
        FileMergeInfo FileMergingInfo { get; set; }

        Task MergeFiles();
    }
}
