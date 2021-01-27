using System;
using System.IO;
using System.Threading;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestTempDirectory
    {
        private static int _instanceCount;

        private readonly Lazy<DirectoryInfo> _ensureTempDirectory;

        internal TestTempDirectory()
        {
            var instanceNumber = Interlocked.Increment(ref _instanceCount);
            _ensureTempDirectory = new Lazy<DirectoryInfo>(() => EnsureTempDirectory(instanceNumber));
        }

        internal FileInfo GetTempFile(string filename)
        {
            var tempFile = new FileInfo(Path.Join(_ensureTempDirectory.Value.FullName, filename));
            tempFile.Delete();
            return tempFile;
        }

        private static DirectoryInfo EnsureTempDirectory(int instanceNumber)
        {
            var tempDir = new DirectoryInfo(Path.Join(Path.GetTempPath(), $"IT{instanceNumber:00000}"));
            if (tempDir.Exists)
            {
                tempDir.Delete(true);
            }
            tempDir.Create();

            return tempDir;
        }
    }
}