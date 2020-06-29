using System;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IDriverWrapper: IDisposable
    {
        internal void AttachDebugInfo(IDriverCleanupContext context);
        internal void Cleanup(IDriverCleanupContext context);
    }
}