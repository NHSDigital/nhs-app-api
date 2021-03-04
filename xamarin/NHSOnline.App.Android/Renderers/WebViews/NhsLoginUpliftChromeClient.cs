using System;
using Android.Webkit;
using Xamarin.Forms.Platform.Android;


namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class NhsLoginUpliftChromeClient : FormsWebChromeClient
    {
        private readonly Action<IValueCallback, FileChooserParams> _callback;

        public NhsLoginUpliftChromeClient(Action<IValueCallback, FileChooserParams> callback)
        {
            _callback = callback;
        }

        public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            _callback(filePathCallback, fileChooserParams);
            return true;
        }
    }
}