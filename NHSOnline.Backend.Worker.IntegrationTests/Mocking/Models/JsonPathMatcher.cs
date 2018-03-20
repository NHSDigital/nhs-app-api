using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class JsonPathMatcher: Matcher
    {
        public JsonPathMatcher(): base("JsonPathMatcher")
        {
        }

        public IList<string> Patterns { get; set; }
    }
}
