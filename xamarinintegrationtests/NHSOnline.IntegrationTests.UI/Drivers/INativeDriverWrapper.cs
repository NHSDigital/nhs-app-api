using NHSOnline.IntegrationTests.UI.Drivers.WebContext;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface INativeDriverWrapper : IDriverWrapper
    {
        WebContextStrategies Web { get; }
        void NhsAppWebViewClosed();
        string AppVersionNumber { get; }
        void Screenshot(string screenshotName);
        void PushTestFile();
        bool VerifyFilePushed();
    }
}