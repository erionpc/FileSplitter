using FileSplitter.Enums;
using FileSplitter.FileMerger;
using FileSplitter.Splitter;

namespace FileSplitter
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