using System;
using System.IO;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal interface IDriverCleanupContext
    {
        string TestName { get; }

        void TryAttach(string name, string filename, Action<FileInfo> createFile);
        void Attach(FileInfo file);

        void TryCleanUp(string description, Action cleanUp);
    }
}