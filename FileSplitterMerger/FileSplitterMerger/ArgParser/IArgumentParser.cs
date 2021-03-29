using FileSplitterMerger.Enums;
using FileSplitterMerger.Merger;
using FileSplitterMerger.Splitter;

namespace FileSplitterMerger.ArgParser
{
    public interface IArgumentParser
    {
        string[] Arguments { get; set; }

        OperationOptionsEnum GetOperation();
        FileSplitInfo BuildFileSplitInfo();
        FileMergeInfo BuildFileMergeInfo();
        bool InfoRequestReceived();
    }
}