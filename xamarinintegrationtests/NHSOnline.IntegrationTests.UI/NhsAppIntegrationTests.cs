using System.Threading.Tasks;

namespace NHSOnline.IntegrationTests.UI
{
    public static class NhsAppIntegrationTests
    {
        public static async Task Initialize()
        {
            Configuration.Initialize();
            await Mocks.Initialize();
        }

        public static async Task Cleanup()
        {
            await Mocks.CleanUp();
        }
    }
}