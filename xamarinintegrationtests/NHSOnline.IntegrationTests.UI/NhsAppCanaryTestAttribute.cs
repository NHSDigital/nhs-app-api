using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    public sealed class NhsAppCanaryTestAttribute: TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories  => new List<string> { "NhsAppCanaryTest" };
    }
}