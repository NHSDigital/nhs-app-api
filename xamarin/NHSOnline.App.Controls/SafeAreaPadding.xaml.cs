using System;
using System.Linq;
using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public class SafeAreaPadding : RoutingEffect
    {
        public const string EffectName = "SafeAreaPaddingEffect";

        public SafeAreaPadding () : base ($"{NhsAppEffects.ResolutionGroupName}.{EffectName}")
        {
        }
    }
}