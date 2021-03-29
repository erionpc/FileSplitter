using FileSplitterMerger.Enums;
using FileSplitterMerger.Merger;
using FileSplitterMerger.Splitter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSplitterMerger.ArgParser
{
    public class ArgumentParser : IArgumentParser
    {
        public string[] Arguments { get; set; }

        public ArgumentParser()
        {
        }

        public OperationOptionsEnum GetOperation()
        {
            var recognisedOperationEnums = Enum.GetValues(typeof(OperationOptionsEnum)).Cast<OperationOptionsEnum>();
            
            if (!recognisedOperationEnums.Select(x => x.GetAttribute<ArgumentInfo>().ArgumentSwitch).Contains(Arguments[0].ToLower()))
                throw new FileSplitterMergerException($"Unrecognised operation. The first argument needs to be one of the following:{Environment.NewLine}" +
                    string.Join(", ", recognisedOperationEnums.Select(x => x.GetAttribute<ArgumentInfo>().ArgumentSwitch)));

            return recognisedOperationEnums.FirstOrDefault(x => x.GetAttribute<ArgumentInfo>().ArgumentSwitch == Arguments[0].ToLower());
        }

        public bool InfoRequestReceived() =>
            Arguments.Any(a => a == OperationOptionsEnum.Info.GetAttribute<ArgumentInfo>().ArgumentSwitch);

        private string GetArgument(string argSwitch)
        {
            string argValue = "";

            int i = 0;
            bool argValueFound = false;
            bool moreValuesForArgument = false;
            while (i < Arguments.Length && (!argValueFound || moreValuesForArgument))
            {
                if (Arguments[i] == argSwitch)
                {
                    if (i + 1 == Arguments.Length)
                        throw new FileSplitterMergerException($"No value supplied for {argSwitch}");

                    if (!Arguments[i + 1].StartsWith("/"))
                    {
                        argValue = Arguments[i + 1].Replace("\"", "");
                        argValueFound = true;

                        if (i + 2 < Arguments.Length && !Arguments[i + 2].StartsWith("/"))
                            moreValuesForArgument = true;
                    }
                }
                else if (moreValuesForArgument && i + 1 < Arguments.Length)
                {
                    if (!Arguments[i + 1].StartsWith("/"))
                        argValue += ", " + Arguments[i + 1].Replace(",", "");
                    else
                        moreValuesForArgument = false;
                }

                i++;
            }

            return argValue;
        }

        public FileSplitInfo BuildFileSplitInfo()
        {
            FileSplitInfo fileSplitInfo = null;

            ArgumentInfo filePathArgument = SplitOptionsEnum.FilePath.GetAttribute<ArgumentInfo>();
            ArgumentInfo numberOfChunksArgument = SplitOptionsEnum.NumberOfChunks.GetAttribute<ArgumentInfo>();
            ArgumentInfo chunkSizeArgument = SplitOptionsEnum.ChunkSize.GetAttribute<ArgumentInfo>();

            var recognisedSwitches = new List<ArgumentInfo>() { filePathArgument, numberOfChunksArgument, chunkSizeArgument };

            var switchesInArgs = Arguments.Where(a => a.StartsWith("/")).Skip(1).ToList();
            var recognisedSwitchesInArgs = switchesInArgs.Where(a => recognisedSwitches.Select(x => x.ArgumentSwitch).Contains(a)).ToList();

            if (recognisedSwitchesInArgs.Count != switchesInArgs.Count)
                throw new FileSplitterMergerException($"Unrecognised split option: {string.Join(", ", switchesInArgs.Except(recognisedSwitchesInArgs))}");

            if ((switchesInArgs?.Count ?? 0) > (switchesInArgs?.Select(x => x?.ToLower())?.Distinct()?.Count() ?? 0))
                throw new FileSplitterMergerException($"Duplicated options");

            string filePath = GetArgument(filePathArgument.ArgumentSwitch);
            if (string.IsNullOrWhiteSpace(filePath))
                throw new FileSplitterMergerException($"{filePathArgument.ArgumentDescription} not specified");

            string numberOfChunksString = GetArgument(numberOfChunksArgument.ArgumentSwitch);
            bool numberOfChunksSpecified = int.TryParse(numberOfChunksString, out int numberOfChunks);

            string chunkSizeString = GetArgument(chunkSizeArgument.ArgumentSwitch);
            bool sizeOfChunksSpecified = long.TryParse(chunkSizeString, out long chunkSize);

            if (!numberOfChunksSpecified && !sizeOfChunksSpecified ||
                (numberOfChunks <= 0 && chunkSize <= 0) ||
                (numberOfChunks > 0 && chunkSize > 0))
                throw new FileSplitterMergerException($"Please specify either {numberOfChunksArgument.ArgumentDescription.ToLower()} or {chunkSizeArgument.ArgumentDescription.ToLower()}");

            if (numberOfChunks > 0)
                fileSplitInfo = new FileSplitInfo(filePath, numberOfChunks);
            else if (chunkSize > 0)
                fileSplitInfo = new FileSplitInfo(filePath, chunkSize);

            return fileSplitInfo;
        }

        public FileMergeInfo BuildFileMergeInfo()
        {
            ArgumentInfo filePartsArgument = MergeOptionsEnum.FileParts.GetAttribute<ArgumentInfo>();
            ArgumentInfo destinationFilePathArgument = MergeOptionsEnum.DestinationFilePath.GetAttribute<ArgumentInfo>();

            var recognisedSwitches = new List<ArgumentInfo>() { filePartsArgument, destinationFilePathArgument };

            var switchesInArgs = Arguments.Where(a => a.StartsWith("/")).Skip(1).ToList();
            var recognisedSwitchesInArgs = switchesInArgs.Where(a => recognisedSwitches.Select(x => x.ArgumentSwitch).Contains(a)).ToList();

            if (recognisedSwitchesInArgs.Count != switchesInArgs.Count)
                throw new FileSplitterMergerException($"Unrecognised merge option: {string.Join(", ", switchesInArgs.Except(recognisedSwitchesInArgs))}");

            if ((switchesInArgs?.Count ?? 0) > (switchesInArgs?.Select(x => x?.ToLower())?.Distinct()?.Count() ?? 0))
                throw new FileSplitterMergerException($"Duplicated options");

            string filePath = GetArgument(destinationFilePathArgument.ArgumentSwitch);
            if (string.IsNullOrWhiteSpace(filePath))
                throw new FileSplitterMergerException($"{destinationFilePathArgument.ArgumentDescription} not specified");

            string filePartsString = GetArgument(filePartsArgument.ArgumentSwitch);
            if (string.IsNullOrWhiteSpace(filePartsString))
                throw new FileSplitterMergerException($"{filePartsArgument.ArgumentDescription} not specified");

            var fileParts = filePartsString.Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().Replace("\"", "")).ToList().AsEnumerable();

            string destinationFilePath = GetArgument(destinationFilePathArgument.ArgumentSwitch);

            return new FileMergeInfo(fileParts, destinationFilePath);
        }
    }
}
