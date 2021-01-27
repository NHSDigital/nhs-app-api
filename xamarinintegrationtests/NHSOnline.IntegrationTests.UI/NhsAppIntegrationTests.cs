using System.Threading.Tasks;

namespace NHSOnline.IntegrationTests.UI
{
    public static class NhsAppIntegrationTests
    {
        public static void Initialize()
        {
            Configuration.Initialize();
            Mocks.Initialize();
        }

        public static async Task Cleanup()
        {
            await Mocks.CleanUp();
        }
    }
}