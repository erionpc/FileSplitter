using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger
{
    public class FileSplitterMergerException : Exception
    {
        public FileSplitterMergerException(string message) : base(message)
        {

        }
    }
}
