using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IDriverWrapper: IDisposable
    {
        internal void AttachDebugInfo(IDriverCleanupContext context);
        internal void Cleanup(IDriverCleanupContext context);
        internal void UpdateBrowserStackStatusToFailed(IDriverCleanupContext context);
        internal void UpdateBrowserStackStatusToPassed(IDriverCleanupContext context);
        internal void WriteTestDetails(string parentJourney, string testName, UnitTestOutcome testOutcome);
        internal void AddBrowserStackSessionDetailsToLogs(IDriverCleanupContext context, TestLogs testLogs);
    }
}