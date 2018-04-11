namespace NHSOnline.Backend.Worker.Mocking.Models
{
    public class WildcardMatcher: Matcher
    {
        public WildcardMatcher(): base("WildcardMatcher")
        {
        }

        public string Pattern { get; set; }
    }
}
