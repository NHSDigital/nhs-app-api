using System.ComponentModel;
using NHSOnline.App.Controls.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NHSOnline.App.iOS.Effects.SafeAreaMarginEffectBottom), SafeAreaMarginBottom.EffectName)]

namespace NHSOnline.App.iOS.Effects
{
    internal class SafeAreaMarginEffectBottom : PlatformEffect
    {
        private Thickness _originalMargin;

        protected override void OnAttached()
        {
            SetMargin();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (args.PropertyName.ToUpperInvariant() == "WIDTH")
            {
                SetMargin();
            }
        }

        private void SetMargin()
        {
            if (UIDevice.CurrentDevice.SupportsSafeAreaInsets() &&
                Element is Layout element)
            {
                _originalMargin = element.Margin;

                var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                element.Margin = CalculatedInsetAdjustedMargin(_originalMargin, insets);
            }
        }

        private static Thickness CalculatedInsetAdjustedMargin(Thickness originalMargin, UIEdgeInsets insets)
        {
            return new Thickness(
                originalMargin.Left + insets.Left,
                originalMargin.Top,
                originalMargin.Right + insets.Right,
                originalMargin.Bottom + insets.Bottom);
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Margin = _originalMargin;
            }
        }
    }
}