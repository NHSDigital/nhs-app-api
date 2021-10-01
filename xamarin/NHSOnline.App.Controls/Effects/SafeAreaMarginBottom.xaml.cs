using Xamarin.Forms;

namespace NHSOnline.App.Controls.Effects
{
    public class SafeAreaMarginBottom : RoutingEffect
    {
        public const string EffectName = "SafeAreaMarginEffectBottom";

        public SafeAreaMarginBottom () : base ($"{NhsAppEffects.ResolutionGroupName}.{EffectName}")
        {
        }
    }
}