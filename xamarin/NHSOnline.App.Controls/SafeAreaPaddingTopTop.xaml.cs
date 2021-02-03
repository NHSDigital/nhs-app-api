using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public class SafeAreaPaddingTop : RoutingEffect
    {
        public const string EffectName = "SafeAreaPaddingEffectTop";

        public SafeAreaPaddingTop () : base ($"{NhsAppEffects.ResolutionGroupName}.{EffectName}")
        {
        }
    }
}