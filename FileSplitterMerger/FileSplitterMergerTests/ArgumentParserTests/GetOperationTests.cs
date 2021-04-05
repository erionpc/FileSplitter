using Xunit;
using FileSplitterMerger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSplitterMerger.Splitter;
using FileSplitterMerger.Enums;
using FileSplitterMerger.Merger;
using FileSplitterMerger.ArgParser;

namespace FileSplitterMerger.Tests.ArgumentParserTests
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
                "test with split argument",
                new string[] { _operation_splitArgument.ArgumentSwitch },
                OperationOptionsEnum.Split,
                null
            };
            yield return new object[]
            {
                "test with split argument uppercase",
                new string[] { _operation_splitArgument.ArgumentSwitch.ToUpper() },
                OperationOptionsEnum.Split,
                null
            };
            yield return new object[]
            {
                "test with merge argument",
                new string[] { _operation_mergeArgument.ArgumentSwitch },
                OperationOptionsEnum.Merge,
                null
            };
            yield return new object[]
            {
                "test with merge argument uppercase",
                new string[] { _operation_mergeArgument.ArgumentSwitch.ToUpper() },
                OperationOptionsEnum.Merge,
                null
            };
            yield return new object[]
            {
                "test with unknown operation argument",
                new string[] { "-p" },
                null,
                new FileSplitterMergerException($"Unrecognised operation. The first argument needs to be one of the following:{Environment.NewLine}" +
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
    }
}