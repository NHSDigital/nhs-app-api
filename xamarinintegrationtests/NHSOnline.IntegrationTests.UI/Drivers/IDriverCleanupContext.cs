using System;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal interface IDriverCleanupContext
    {
        string TestName { get; }
        void TryAttach(string name, Func<string> createFile);
        void TryCleanUp(string description, Action cleanUp);
    }
}