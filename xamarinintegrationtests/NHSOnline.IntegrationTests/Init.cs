using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests
{
    [TestClass]
    public static class Init
    {
        [AssemblyInitialize]
#pragma warning disable CA1801 // Remove unused parameter
        public static void Initialize(TestContext context)
#pragma warning restore CA1801 // Remove unused parameter
        {
            NhsAppIntegrationTests.Initialize();
        }

        [AssemblyCleanup]
        public static async Task CleanUp()
        {
            await NhsAppIntegrationTests.Cleanup();
        }
    }
}