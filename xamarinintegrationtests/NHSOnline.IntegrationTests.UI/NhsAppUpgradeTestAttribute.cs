using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    public sealed class NhsAppUpgradeTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories  => new List<string> { "NhsAppUpgradeTest" };
    }
}