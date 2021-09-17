namespace NHSOnline.App.DependencyServices
{
    public abstract class DownloadFileResult
    {
        private DownloadFileResult()
        {
        }

        public sealed class Success: DownloadFileResult
        {
        }

        public sealed class Failed: DownloadFileResult
        {
        }
    }
}