using FileSplitterMerger.ArgParser;

namespace FileSplitterMerger.Enums
{
    public enum MergeOptionsEnum
    {
        [ArgumentInfo("/p", "File parts")]
        FileParts,
        [ArgumentInfo("/d", "Destination file path")]
        DestinationFilePath
    }
}
