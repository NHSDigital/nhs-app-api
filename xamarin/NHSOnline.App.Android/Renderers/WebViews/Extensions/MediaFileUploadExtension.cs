using System;
using Android.Webkit;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Logging;
using Xamarin.Forms.Platform.Android;
using Exception = Java.Lang.Exception;
using WebView = Xamarin.Forms.WebView;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class MediaFileUploadExtension : WebViewRendererExtension
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<MediaFileUploadExtension>();

        private IFileUploadAwareWebView? _fileUploadAwareWebView;

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.NewElement is IFileUploadAwareWebView fileUploadAwareWebView)
            {
                _fileUploadAwareWebView = fileUploadAwareWebView;
            }
            else if (e.NewElement != null)
            {
                throw new NotSupportedException(
                    $"The {nameof(MediaFileUploadExtension)} extension can only be added to WebViews that implement {nameof(IFileUploadAwareWebView)}");
            }
        }

        internal override ShowFileChooserExtensionDecision OnShowFileChooser(
            Android.Webkit.WebView _,
            IValueCallback filePathCallback,
            WebChromeClient.FileChooserParams fileChooserParams)
        {
            try
            {
                var request = new SelectMediaRequest(filePathCallback, fileChooserParams);
                _fileUploadAwareWebView!.SelectMediaCommand.Execute(request);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to execute SelectMediaCommand");
                filePathCallback.OnReceiveValue(null);
            }

            return ShowFileChooserExtensionDecision.Handled;
        }
    }
}