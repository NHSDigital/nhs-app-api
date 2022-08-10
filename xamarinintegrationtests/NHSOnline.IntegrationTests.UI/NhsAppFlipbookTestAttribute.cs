using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    public sealed class NhsAppFlipbookTestAttribute: TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories  => new List<string> { "NhsAppFlipbookTest" };

        public string? FlipbookTestName { get; set; }

        public string? ParentJourney { get; set; }

        public string? JourneyId { get; set; }
    }
}