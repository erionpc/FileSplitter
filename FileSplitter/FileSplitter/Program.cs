using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var argParser = new ArgumentParser(args);
                var fileSplitInfo = argParser.BuildFileSplitInfo();

                if (argParser.InfoRequestReceived())
                {
                    Console.WriteLine($"{Environment.NewLine}Supported syntax:{Environment.NewLine}{PrintOptions()}");
                }
                else
                {
                    Console.WriteLine($"{Environment.NewLine}{fileSplitInfo}{Environment.NewLine}" +
                                      $"{Environment.NewLine}To do: file splitting");
                }
            }
            catch (FileSplitException ex)
            {
                Console.WriteLine($"{Environment.NewLine}{ex.Message}{Environment.NewLine}");
                Console.WriteLine($"Supported syntax:{Environment.NewLine}{PrintOptions()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Environment.NewLine}{ex}");
            }
        }

        private static string PrintOptions()
        {
            string options = "";
            foreach (SwitchEnum switchOption in Enum.GetValues(typeof(SwitchEnum)))
            {
                var argumentInfo = switchOption.GetAttribute<ArgumentInfo>();
                options += $"{argumentInfo.ArgumentSwitch} = {argumentInfo.ArgumentDescription}{Environment.NewLine}";
            }

            return options;
        }
    }
}
