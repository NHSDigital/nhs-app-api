using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.DependencyServices
{
    public interface IFileHandler
    {
        Task<DownloadFileResult> DownloadFile(DownloadRequest downloadRequest);
    }
}