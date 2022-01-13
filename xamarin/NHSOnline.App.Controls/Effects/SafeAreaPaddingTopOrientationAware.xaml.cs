using Xamarin.Forms;

namespace NHSOnline.App.Controls.Effects
{
    public class SafeAreaPaddingTopOrientationAware : RoutingEffect
    {
        public const string EffectName = "SafeAreaPaddingTopOrientationAware";

        public SafeAreaPaddingTopOrientationAware () : base ($"{NhsAppEffects.ResolutionGroupName}.{EffectName}")
        {
        }
    }
}