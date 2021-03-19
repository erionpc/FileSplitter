namespace FileSplitter
{
    public interface IArgumentParser
    {
        string[] Arguments { get; }

        FileSplitInfo BuildFileSplitInfo();
        bool InfoRequestReceived();
    }
}