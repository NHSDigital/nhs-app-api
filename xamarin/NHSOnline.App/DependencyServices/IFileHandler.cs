using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.DependencyServices
{
    public interface IFileHandler
    {
        void StoreFileInDownloads(DownloadRequest downloadRequest);

        void HandleFile(DownloadRequest downloadRequest);
    }
}