using System;
using System.Text.RegularExpressions;

namespace NHSOnline.Backend.Support.Logging
{
    [Serializable]
    public class LogCensorFilter
    {
        public string Match { get; set; }
        public string Replacement { get; set; }

        public string CensorContent(string state)
        {
            return Regex.Replace(state, Match, Replacement);
        }
    }
}
