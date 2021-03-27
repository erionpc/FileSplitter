namespace FileSplitter.Enums
{
    public enum SplitOptionsEnum
    {
        [ArgumentInfo("/f", "File path")]
        FilePath,
        [ArgumentInfo("/n", "Split in number of chunks")]
        NumberOfChunks,
        [ArgumentInfo("/s", "Split in chunks of size in bytes")]
        ChunkSize
    }
}
