using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyServices
{
    public interface IFileHandler
    {
        Task<DownloadFileResult> DownloadFile(DownloadRequest downloadRequest, View webViewElement);
    }
}