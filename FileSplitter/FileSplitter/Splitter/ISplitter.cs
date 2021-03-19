using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileSplitter.Splitter
{
    public interface ISplitter
    {
        List<string> CreatedFiles { get; }

        Task Split();
    }
}