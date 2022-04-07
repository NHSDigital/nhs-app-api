using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    public class FlipbookTestDetails
    {
        public string? AppVersion { get; set; }
        public string? Device { get; set; }
        public string? OSVersion { get; set; }
        public string? ParentJourney { get; set; }
        public string? TestName { get; set; }

        public string? TestId { get; set; }

        public string? Folder { get; set; }
        public UnitTestOutcome? TestOutcome { get; set; }
    }
}