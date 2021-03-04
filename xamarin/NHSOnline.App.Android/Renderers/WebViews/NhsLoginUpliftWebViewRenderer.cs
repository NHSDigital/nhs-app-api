using System;
using System.Collections.Generic;
using Android.Content;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(NhsLoginUpliftWebView), typeof(NhsLoginUpliftWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsLoginUpliftWebViewRenderer: WebViewRenderer
    {
        private readonly List<IWebViewRendererExtension> _extensions;

        public NhsLoginUpliftWebViewRenderer(Context context) : base(context)
        {
            _extensions = new List<IWebViewRendererExtension>
            {
                new UserAgentWebViewRendererExtension(this),
                new EnableTargetBlankLinksRendererExtension(this)
            };
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

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            foreach (var extension in _extensions)
            {
                extension.OnElementChanged(e);
            }
        }
    }
}
