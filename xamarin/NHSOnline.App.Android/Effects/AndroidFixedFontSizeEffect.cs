using System;
using Android.Widget;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.Effects;
using NHSOnline.App.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(NHSOnline.App.Droid.Effects.AndroidFixedFontSizeEffect), nameof(FixedFontSizeEffect))]

namespace NHSOnline.App.Droid.Effects
{
    internal sealed class AndroidFixedFontSizeEffect : PlatformEffect
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<AndroidFixedFontSizeEffect>();

        protected override void OnAttached()
        {
            try
            {
                if (Control is TextView textView &&
                    Element is Label label &&
                    FixedFontSizeEffect.GetHasFixedFontSize(Element))
                {
                    textView.SetTextSize(Android.Util.ComplexUnitType.Dip, (float) label.FontSize);
                    PreventFontSizeResetting(label);
                    label.Scale = 1;
                }
            }
            catch (Exception e)
            {
                Logger.LogDebug("Exception occurred", e);
            }
        }

        private static void PreventFontSizeResetting(Label label)
        {
            label.FontSize = Double.NaN;
        }

        protected override void OnDetached()
        {
        }
    }
}