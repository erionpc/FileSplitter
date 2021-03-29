using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileSplitterMergerTests.SplitterMergerTests
{
    public class SplitterMergerTestsBase
    {
        protected IConfiguration Configuration;

        public SplitterMergerTestsBase() 
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"SplitterConfig:Buffersize", "4096"}
            };

            Configuration = new ConfigurationBuilder()
                                    .AddInMemoryCollection(inMemorySettings)
                                    .Build();
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
