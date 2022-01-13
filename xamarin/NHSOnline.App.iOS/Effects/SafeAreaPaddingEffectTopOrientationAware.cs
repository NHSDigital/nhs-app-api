using System.ComponentModel;
using NHSOnline.App.Controls.Effects;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NHSOnline.App.iOS.Effects.SafeAreaPaddingEffectTopOrientationAware), SafeAreaPaddingTopOrientationAware.EffectName)]

namespace NHSOnline.App.iOS.Effects
{
    internal class SafeAreaPaddingEffectTopOrientationAware : PlatformEffect
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
            if (UIDevice.CurrentDevice.SupportsSafeAreaInsets() &&
                Element is Layout element)
            {
                _originalPadding = element.Padding;

                if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                {
                    var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                    element.Padding = new Thickness(0, insets.Top, 0, 0);
                }
                else
                {
                    var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                    element.Padding = new Thickness(insets.Left, insets.Top, insets.Right, 0);
                }
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