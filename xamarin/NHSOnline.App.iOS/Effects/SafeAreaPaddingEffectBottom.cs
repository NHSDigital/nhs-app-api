using System;
using NHSOnline.App.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NHSOnline.App.iOS.Effects.SafeAreaPaddingEffectBottom), SafeAreaPaddingBottom.EffectName)]

namespace NHSOnline.App.iOS.Effects
{
    internal class SafeAreaPaddingEffectBottom : PlatformEffect
    {
        private Thickness _originalPadding;

        protected override void OnAttached()
        {
            if (UIDevice.CurrentDevice.SupportsSafeAreaInsets() &&
                Element is Layout element)
            {
                _originalPadding = element.Padding;

                var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                element.Padding = AddBottomPadding(element, insets.Bottom);
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _originalPadding;
            }
        }

        private static Thickness AddBottomPadding(
            Layout element,
            nfloat BottomInsetPadding)
        {
            var (left, top, right, bottom) = element.Padding;
            return new Thickness(
                left,
                top,
                right,
                bottom + BottomInsetPadding);
        }
    }
}