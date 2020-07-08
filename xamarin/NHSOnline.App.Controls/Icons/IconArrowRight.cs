using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace NHSOnline.App.Controls.Icons
{
    public class IconArrowRight: SvgImage
    {
        public IconArrowRight() : base($"{nameof(Icons)}.icon-arrow-right.svg")
        {}
    }
}