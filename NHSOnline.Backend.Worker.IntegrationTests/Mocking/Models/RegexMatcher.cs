using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class RegexMatcher: Matcher
    {
        public RegexMatcher(): base("RegexMatcher")
        {
        }

        public IList<string> Patterns { get; set; }
    }
}
