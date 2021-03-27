using Xunit;
using FileSplitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSplitter.Splitter;
using FileSplitter.Enums;
using FileSplitter.FileMerger;

namespace FileSplitter.Tests.ArgumentParserTests
{
    public class GetOperationTests
    {
        static readonly ArgumentInfo _operation_infoArgument = OperationOptionsEnum.Info.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _operation_splitArgument = OperationOptionsEnum.Split.GetAttribute<ArgumentInfo>();
        static readonly ArgumentInfo _operation_mergeArgument = OperationOptionsEnum.Merge.GetAttribute<ArgumentInfo>();

        public static IEnumerable<object[]> GetOperation_TestData()
        {
            yield return new object[]
            {
                "test with /s",
                new string[] { "/s" },
                OperationOptionsEnum.Split,
                null
            };
            yield return new object[]
            {
                "test with /S",
                new string[] { "/S" },
                OperationOptionsEnum.Split,
                null
            };
            yield return new object[]
            {
                "test with /m",
                new string[] { "/m" },
                OperationOptionsEnum.Merge,
                null
            };
            yield return new object[]
            {
                "test with /M",
                new string[] { "/M" },
                OperationOptionsEnum.Merge,
                null
            };
            yield return new object[]
            {
                "test with /p",
                new string[] { "/p" },
                null,
                new FileSplitException($"Unrecognised operation. The first argument needs to be one of the following:{Environment.NewLine}" +
                    string.Join(", ", Enum.GetValues(typeof(OperationOptionsEnum)).Cast<OperationOptionsEnum>().Select(x => x.GetAttribute<ArgumentInfo>().ArgumentSwitch)))
            };
        }

        [Theory]
        [MemberData(nameof(GetOperation_TestData))]
        public void GetOperationTest(string testCase, string[] arguments, OperationOptionsEnum expectedResult, Exception expectedException)
        {
            try
            {
                var argumentParser = new ArgumentParser
                {
                    Arguments = arguments
                };
                var actual = argumentParser.GetOperation();

                Assert.NotNull(testCase);
                Assert.Equal(expectedResult, actual);
            }
            catch (Exception ex)
            {
                Assert.Equal(expectedException.GetType().FullName, ex.GetType().FullName);
                Assert.Equal(expectedException.Message, ex.Message);
            }
        }

        // Todo: Add unit tests for NumberOfChunkSplitter
        // Todo: Add unit tests for SizeOfChunkSplitter
        // Todo: Add unit tests for Merger
    }
}