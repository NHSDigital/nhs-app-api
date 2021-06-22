using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.DependencyServices
{
    public interface IFileSystemService
    {
        void StoreFileInDownloads(DownloadRequest downloadRequest);

        void PresentFileActionController(DownloadRequest downloadRequest);
    }
}