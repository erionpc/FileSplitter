using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter.FileMerger
{
    public class FileMergeInfo
    {
        private readonly IEnumerable<string> _fileParts;
        private readonly string _destinationFilePath;

        public FileMergeInfo(IEnumerable<string> fileParts, string destinationFilePath)
        {
            _fileParts = fileParts;
            _destinationFilePath = destinationFilePath;
        }

        public IEnumerable<string> FileParts { get => _fileParts; }
        public string DestinationFile { get => _destinationFilePath; }

        public override string ToString() =>
            $"{nameof(FileMergeInfo)}:{Environment.NewLine}" + 
            $"{nameof(FileParts)} = {string.Join(", ", FileParts)}{Environment.NewLine}" +
            $"{nameof(DestinationFile)} = {DestinationFile}";
    }
}
