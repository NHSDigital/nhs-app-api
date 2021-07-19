using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.DependencyServices
{
    public interface IFileHandler
    {
        Task StoreFileInDownloads(DownloadRequest downloadRequest);

        Task HandleFile(DownloadRequest downloadRequest);
    }
}