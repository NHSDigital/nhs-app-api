using System;
using Android.Webkit;


namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class NhsLoginUpliftChromeClient : BaseChromeClient
    {
        private readonly Action<IValueCallback, FileChooserParams> _fileChooserCallback;

        public NhsLoginUpliftChromeClient(Action<IValueCallback, FileChooserParams> fileChooserCallback)
        {
            _fileChooserCallback = fileChooserCallback;
        }

        public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            _fileChooserCallback(filePathCallback, fileChooserParams);
            return true;
        }
    }
}