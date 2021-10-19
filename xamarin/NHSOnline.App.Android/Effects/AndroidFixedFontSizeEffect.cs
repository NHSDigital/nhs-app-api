using System;
using System.ComponentModel;
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
                SetFontSizeAndPreventScaling();
            }
            catch (Exception e)
            {
                Logger.LogDebug("Exception occurred", e);
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            // If change screen orientation, ensure font size does not change by re-applying effect.
            if (args.PropertyName.ToUpperInvariant() == "WIDTH")
            {
                try
                {
                    SetFontSizeAndPreventScaling();
                }
                catch (Exception e)
                {
                    Logger.LogDebug("Exception occurred", e);
                }
            }
        }

        private void SetFontSizeAndPreventScaling()
        {
            if (Control is TextView textView &&
                Element is Label label &&
                FixedFontSizeEffect.GetHasFixedFontSize(Element))
            {
                textView.SetTextSize(Android.Util.ComplexUnitType.Dip, (float)label.FontSize);
                PreventFontSizeResetting(label);
                label.Scale = 1;
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