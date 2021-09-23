using System.Threading.Tasks;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IIOSDriverWrapper : INativeDriverWrapper, IIOSInteractor
    {
        WaitForAction SwipeBack();
        void PushTestFile();
        Task DisableNetwork();
        Task ResetNetwork();
        void CloseApp();
        void LaunchApp();
        void AssertRunningInForeground();
    }
}