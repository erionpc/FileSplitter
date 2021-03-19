using System;
using System.Linq;

namespace FileSplitter
{
    public class ArgumentParser : IArgumentParser
    {
        public string[] Arguments { get; set; }

        public ArgumentParser()
        {
        }

        public bool InfoRequestReceived() =>
            Arguments.Any(a => a == SwitchEnum.Info.GetAttribute<ArgumentInfo>().ArgumentSwitch);

        private string GetArgument(string argSwitch)
        {
            string argValue = "";

            int i = 0;
            bool argFound = false;
            while (i < Arguments.Length && !argFound)
            {
                if (Arguments[i] == argSwitch)
                {
                    if (i + 1 == Arguments.Length)
                        throw new FileSplitException($"No value supplied for {argSwitch}");

                    if (!Arguments[i + 1].StartsWith("/"))
                    {
                        argValue = Arguments[i + 1];
                        argFound = true;
                    }
                }

                i++;
            }

            return argValue;
        }

        public FileSplitInfo BuildFileSplitInfo()
        {
            FileSplitInfo fileSplitInfo = null;

            var recognisedSwitches = Enum.GetValues(typeof(SwitchEnum)).Cast<SwitchEnum>().Select(x => x.GetAttribute<ArgumentInfo>()).ToList();
            ArgumentInfo filePathArgument = SwitchEnum.FilePath.GetAttribute<ArgumentInfo>();
            ArgumentInfo numberOfChunksArgument = SwitchEnum.NumberOfChunks.GetAttribute<ArgumentInfo>();
            ArgumentInfo chunkSizeArgument = SwitchEnum.ChunkSize.GetAttribute<ArgumentInfo>();
            ArgumentInfo infoArgument = SwitchEnum.Info.GetAttribute<ArgumentInfo>();

            var switchesInArgs = Arguments.Where(a => a.StartsWith("/")).ToList();
            var recognisedSwitchesInArgs = switchesInArgs.Where(a => recognisedSwitches.Select(x => x.ArgumentSwitch).Contains(a)).ToList();

            if (recognisedSwitchesInArgs.Count != switchesInArgs.Count)
                throw new FileSplitException($"Unrecognised switch: {string.Join(", ", switchesInArgs.Except(recognisedSwitchesInArgs))}");

            if ((switchesInArgs?.Count ?? 0) > (switchesInArgs?.Select(x => x?.ToLower())?.Distinct()?.Count() ?? 0))
                throw new FileSplitException($"Duplicated switches");

            if (!InfoRequestReceived())
            {
                string filePath = GetArgument(filePathArgument.ArgumentSwitch);
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new FileSplitException($"{filePathArgument.ArgumentDescription} not specified");

                string numberOfChunksString = GetArgument(numberOfChunksArgument.ArgumentSwitch);
                int.TryParse(numberOfChunksString, out int numberOfChunks);

                string chunkSizeString = GetArgument(chunkSizeArgument.ArgumentSwitch);
                long.TryParse(chunkSizeString, out long chunkSize);

                if ((string.IsNullOrWhiteSpace(numberOfChunksString) && string.IsNullOrWhiteSpace(chunkSizeString)) ||
                    (numberOfChunks <= 0 && chunkSize <= 0) ||
                    (numberOfChunks > 0 && chunkSize > 0))
                    throw new FileSplitException($"Please specify either {numberOfChunksArgument.ArgumentDescription.ToLower()} or {chunkSizeArgument.ArgumentDescription.ToLower()}");

                if (numberOfChunks > 0)
                    fileSplitInfo = new FileSplitInfo(filePath, numberOfChunks);
                else if (chunkSize > 0)
                    fileSplitInfo = new FileSplitInfo(filePath, chunkSize);
            }

            return fileSplitInfo;
        }
    }
}
