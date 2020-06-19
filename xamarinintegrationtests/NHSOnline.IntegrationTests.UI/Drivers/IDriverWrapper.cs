using System;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IDriverWrapper: IDisposable
    {
        void AttachDebugInfo(IDriverCleanupContext context);
        void Cleanup(IDriverCleanupContext context);
    }
}