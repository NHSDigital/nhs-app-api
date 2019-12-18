using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NHSOnline.Backend.Support.ResponseParsers
{
    public class RedexPatternRedactor
    {
        private readonly IDictionary<string, string> _patternAndRedactors;

        public RedexPatternRedactor(IDictionary<string, string> patternAndRedactors)
        {
            _patternAndRedactors = patternAndRedactors;
        }

        public string RedactOrNull(string input)
        {
            foreach (var patternAndRedactor in _patternAndRedactors)
            {
                if (Regex.IsMatch(input, patternAndRedactor.Key))
                {
                    return Regex.Replace(input, patternAndRedactor.Key, patternAndRedactor.Value);
                }
            }

            return null;
        }
    }
}