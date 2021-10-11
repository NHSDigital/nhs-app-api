using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.Controls.WebViews
{
    public interface IFileUploadAwareWebView
    {
        AsyncCommand<ISelectMediaRequest> SelectMediaCommand { get; }
    }
}