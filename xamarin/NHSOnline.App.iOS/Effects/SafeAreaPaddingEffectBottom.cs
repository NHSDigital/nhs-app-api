using System.ComponentModel;
using NHSOnline.App.Controls.Effects;
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
            SetPadding();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (args.PropertyName.ToUpperInvariant() == "WIDTH")
            {
                SetPadding();
            }
        }

        private void SetPadding()
        {
            if (Element is Layout element)
            {
                _originalPadding = element.Padding;

                var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                element.Padding = new Thickness(insets.Left, 0, insets.Right, insets.Bottom);
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _originalPadding;
            }
        }
    }
}