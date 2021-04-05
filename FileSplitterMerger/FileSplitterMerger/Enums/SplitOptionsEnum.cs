using FileSplitterMerger.ArgParser;

namespace FileSplitterMerger.Enums
{
    public enum SplitOptionsEnum
    {
        [ArgumentInfo("-f", "File path")]
        FilePath,
        [ArgumentInfo("-d", "Destination path")]
        DestinationPath,
        [ArgumentInfo("-n", "Split in number of chunks")]
        NumberOfChunks,
        [ArgumentInfo("-s", "Split in chunks of size in bytes")]
        ChunkSize
    }
}
