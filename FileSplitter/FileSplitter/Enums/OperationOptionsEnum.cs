namespace FileSplitter.Enums
{
    public enum OperationOptionsEnum
    {
        [ArgumentInfo("/s", "File split")]
        Split,
        [ArgumentInfo("/m", "File merge")]
        Merge,
        [ArgumentInfo("/i", "Syntax info")]
        Info
    }
}