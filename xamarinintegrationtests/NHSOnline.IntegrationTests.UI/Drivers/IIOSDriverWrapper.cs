using System.Threading.Tasks;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IIOSDriverWrapper : INativeDriverWrapper, IIOSInteractor
    {
        WaitForAction SwipeBack();
        WaitForAction PressHome();
        WaitForAction SwipeToNextScreen();
        void PushTestFile();
        Task DisableNetwork();
        void BackgroundApp();
    }
}