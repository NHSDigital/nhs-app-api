using System;
using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NhsLoginUpliftWebView), typeof(NhsLoginUpliftWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    [SuppressMessage("Reliability", "CA2000", Justification = "IProoveExtension is disposed of in the base classes dispose method")]
    public sealed class NhsLoginUpliftWebViewRenderer: BaseWebViewRenderer
    {
        public NhsLoginUpliftWebViewRenderer(Context context) : base(context)
        {
            AddExtension(new UserAgentWebViewRendererExtension(this));
            AddExtension(new EnableTargetBlankLinksRendererExtension(this));
            AddExtension(new IProovExtension(this));
        }

        protected override FormsWebChromeClient GetFormsWebChromeClient() => new NhsLoginUpliftChromeClient(ShowFileChooser);

        private void ShowFileChooser(IValueCallback valueCallback, WebChromeClient.FileChooserParams fileChooserParams)
        {
            try
            {
                var request = new SelectMediaRequest(valueCallback, fileChooserParams);
                ((NhsLoginUpliftWebView) Element).SelectMediaCommand.Execute(request);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                valueCallback.OnReceiveValue(null);
            }
        }
    }
}
