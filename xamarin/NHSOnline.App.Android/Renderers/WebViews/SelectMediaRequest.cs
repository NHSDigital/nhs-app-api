using System;
using System.Linq;
using Android.Webkit;
using Java.IO;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using Uri = Android.Net.Uri;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    sealed class SelectMediaRequest : ISelectMediaRequest
    {
        private readonly IValueCallback _valueCallback;
        internal SelectMediaRequest(IValueCallback valueCallback, WebChromeClient.FileChooserParams fileChooserParams)
        {
            _valueCallback = valueCallback;

            var acceptTypes = fileChooserParams.GetAcceptTypes() ?? Array.Empty<string>();
            Type = acceptTypes.Any(t => t.StartsWith("video/", StringComparison.OrdinalIgnoreCase)) switch
            {
                true => SelectMediaType.Video,
                false => SelectMediaType.Image
            };

            Mode = fileChooserParams.IsCaptureEnabled switch
            {
                true => SelectMediaMode.Take,
                false => SelectMediaMode.Pick
            };
        }

        public SelectMediaType Type { get; }
        public SelectMediaMode Mode { get; }

        public void MediaSelected(string path)
        {
            using var imageFile = new File(path);
            var imageUri = Uri.FromFile(imageFile);

            if (imageUri is null)
            {
                _valueCallback.OnReceiveValue(null);
            }
            else
            {
                Uri[] images = { imageUri };
                _valueCallback.OnReceiveValue(images);
            }

        }

        public void NoMediaSelected()
        {
            _valueCallback.OnReceiveValue(null);
        }
    }
}