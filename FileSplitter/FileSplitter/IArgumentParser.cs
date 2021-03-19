namespace FileSplitter
{
    public interface IArgumentParser
    {
        string[] Arguments { get; set; }

        FileSplitInfo BuildFileSplitInfo();
        bool InfoRequestReceived();
    }
}