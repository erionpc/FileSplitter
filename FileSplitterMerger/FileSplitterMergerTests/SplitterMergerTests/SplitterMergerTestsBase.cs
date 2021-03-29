using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
    }
}
