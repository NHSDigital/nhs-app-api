using System;
using System.Threading.Tasks;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IIOSDriverWrapper : INativeDriverWrapper, IIOSInteractor
    {
        WaitForAction SwipeBack();
        Task DisableNetwork();
        Task ResetNetwork();
        Task ResetNetworkAndWait(TimeSpan timeSpan);
        void CloseApp();
        void LaunchApp();
        void AssertRunningInForeground();
    }
}