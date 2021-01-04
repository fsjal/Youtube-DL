using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Youtube_DL.Entities.Link;

namespace Youtube_DL.Models
{
    static class DownloadClient
    {
        public delegate void OnInfoParsedHandler(Information info, string content);

        public static event OnInfoParsedHandler OnInfoParsed;

        public static async Task downloadAsync(ProcessCommand command, CancellationToken token)
        {
            await foreach (string line in command.ResultAsync(token))
            {
                process(line);
            }
        }

        private static void process(string line)
        {
            Parser.GetName(line)?.Notify();
            Parser.GetStats(line)?.Notify();
            Parser.DetectMerge(line)?.Notify();
            Parser.DetectError(line)?.Notify();
        }

        public static void Notify(this Dictionary<Information, string> dict)
        {
            foreach(var (key, value) in dict) OnInfoParsed?.Invoke(key, value);
        }
    }
}
