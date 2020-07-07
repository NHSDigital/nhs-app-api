using System;
using System.Linq;
using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public class SafeAreaPaddingBottom : RoutingEffect
    {
        public const string EffectName = "SafeAreaPaddingEffectBottom";

        public SafeAreaPaddingBottom () : base ($"{NhsAppEffects.ResolutionGroupName}.{EffectName}")
        {
        }
    }
}