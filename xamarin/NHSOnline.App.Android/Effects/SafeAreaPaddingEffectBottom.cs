using Android.Content.Res;
using NHSOnline.App.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName(NhsAppEffects.ResolutionGroupName)]
[assembly: ExportEffect(typeof(NHSOnline.App.Droid.Effects.SafeAreaPaddingEffectBottom), SafeAreaPaddingBottom.EffectName)]
namespace NHSOnline.App.Droid.Effects
{
    internal class SafeAreaPaddingEffectBottom : PlatformEffect
    {
        private Thickness _originalPadding;
        private const int AndroidSoftNavigationPadding = 30;

        protected override void OnAttached()
        {
            if (!(Element is Layout element))
            {
                return;
            }

            _originalPadding = element.Padding;
            if (ShouldApplyBottomPadding())
            {
                element.Padding = AddBottomPadding(element, AndroidSoftNavigationPadding);
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _originalPadding;
            }
        }

        private static bool ShouldApplyBottomPadding()
        {
            // This will return false for the boolean on a simulator and will ONLY work on a physical device
            var id = Resources.System.GetIdentifier("config_showNavigationBar", "bool", "android");
            return id > 0 && Resources.System.GetBoolean(id);
        }

        private static Thickness AddBottomPadding(
            Layout element,
            int BottomInsetPadding)
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