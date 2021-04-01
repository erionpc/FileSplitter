using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileSplitterMergerTests.SplitterMergerTests
{
    public class SplitterMergerTestsBase
    {
        protected IConfiguration Configuration;
        private static string _assemblyDirectory;

        public SplitterMergerTestsBase() 
        {
            var inMemorySettings = new Dictionary<string, string> 
            {
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
                if (string.IsNullOrEmpty(_assemblyDirectory))
                {
                    string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    UriBuilder uri = new UriBuilder(assemblyLocation);
                    string path = Uri.UnescapeDataString(uri.Path);
                    _assemblyDirectory = Path.GetDirectoryName(path);
                }
                return _assemblyDirectory;
            }
        }
    }
}
