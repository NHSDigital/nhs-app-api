namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class WildcardMatcher: Matcher
    {
        public WildcardMatcher(): base("WildcardMatcher")
        {
        }

        public string Pattern { get; set; }
    }
}
