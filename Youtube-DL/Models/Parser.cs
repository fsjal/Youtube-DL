using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Youtube_DL.Entities.Link;

namespace Youtube_DL.Models
{
    static class Parser
    {
        public static Dictionary<Information, string> GetName(string input)
        {
            var result = new Regex(@"^\[download\] Destination: (.*)$").Match(input).Groups[1].Value;
            if (result == "") return null;
            return new Dictionary<Information, string>() { { Information.Name, result } };
        }

        public static Dictionary<Information, string> GetStats(string input)
        {
            var infos = new Regex(@"^\[download\]\s+(.*?)% of\s+(.*?) at\s+(.*?) ETA\s+(.*)").Match(input).Groups;
            if (infos.Count == 0) return null;
            return new Dictionary<Information, string>() {
                { Information.Progress  , infos[1].Value.Trim() == "" ? "0.0" : infos[1].Value.Trim()},
                { Information.Size      , infos[2].Value.Trim() },
                { Information.Speed     , infos[4].Value.Trim() == "00:00" ? "" : infos[3].Value.Trim() },
                { Information.Eta       , infos[4].Value.Trim() == "00:00" ? "Finised" : infos[4].Value.Trim() },     
            };
        }
        
        public static Dictionary<Information, string> DetectMerge(string input)
        {
            var info = new Regex("Merging formats into \"(.+)\"$").Match(input).Groups[1].Value;
            if (info == "") return null;
            return new Dictionary<Information, string>() { { Information.Name, info } };
        }

        public static Dictionary<Information, string> DetectError(string input)
        {
            var info = new Regex("ERROR: (.*)").Match(input).Groups[1].Value;
            if (info == "") return null;
            return new Dictionary<Information, string>() { { Information.Error, info } };
        }

    }   
}
