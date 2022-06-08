using System;
using System.Threading.Tasks;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidDriverWrapper : INativeDriverWrapper, IAndroidInteractor
    {
        WaitForAction PressBackButton();
        void AssertNotRunningInForeground();
        void AssertRunningInForeground();
        Task EnableAirplaneMode();
        Task ResetNetwork();
        Task ResetNetworkAndWait(TimeSpan timeSpan);
        void DismissKeyboard();
        void SendKey(int key);
        void CloseApp();
        void BackgroundApp();
        AndroidChromeApp OpenChromeApp();
        void VerifyChromeAppUrl(IAndroidDriverWrapper driver, string expectedDestination);
        void LaunchApp();
    }
}