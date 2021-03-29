using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger.ArgParser
{
    [AttributeUsage(AttributeTargets.All)]
    public class ArgumentInfo : DescriptionAttribute
    {
        public ArgumentInfo(string argumentSwitch, string argumentDescription)
        {
            this.ArgumentSwitch = argumentSwitch;
            this.ArgumentDescription = argumentDescription;
        }

        public string ArgumentSwitch { get; set; }
        public string ArgumentDescription { get; set; }
    }
}
