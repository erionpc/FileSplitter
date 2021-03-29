using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileSplitterMerger.Splitter
{
    public interface ISplitter
    {
        SplitterType TypeOfSplitter { get; protected set; }

        List<string> CreatedFiles { get; }

        FileSplitInfo FileSplittingInfo { get; set; }

        Task Split();
    }
}