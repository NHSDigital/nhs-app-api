using System;
using NHSOnline.App.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SafeAreaPaddingEffect = NHSOnline.App.iOS.Effects.SafeAreaPaddingEffect;

[assembly: ResolutionGroupName(NhsAppEffects.ResolutionGroupName)]
[assembly: ExportEffect(typeof(SafeAreaPaddingEffect), SafeAreaPadding.EffectName)]

namespace NHSOnline.App.iOS.Effects
{
    internal class SafeAreaPaddingEffect : PlatformEffect
    {
        private Thickness _originalPadding;
        protected override void OnAttached()
        {
            if (!(Element is Layout element))
            {
                return;
            }

            _originalPadding = element.Padding;

            var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
            element.Padding = AddTopPadding(element, insets.Top > 0 ? insets.Top : 20);
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _originalPadding;
            }
        }

        private static Thickness AddTopPadding(Layout element, nfloat topInsetPadding)
        {
            var (left, top, right, bottom) = element.Padding;
            return new Thickness(
                left,
                top + topInsetPadding,
                right,
                bottom);
        }
    }
}