using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace NHSOnline.App.Controls
{
    public class SvgImage : ContentView
    {
        private readonly string? _resourceName;

        private readonly SKSize _svgSize;

        internal SvgImage(string svgPath)
        {
            _resourceName = $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{svgPath}";

            var canvasView = new SKCanvasView();
            canvasView.PaintSurface += (sender, args) => PaintBackground(args.Info, args.Surface);
            Content = canvasView;

            _svgSize = LoadSvg().CullRect.Size;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (_svgSize.Height < float.Epsilon || _svgSize.Width < float.Epsilon)
            {
                return new SizeRequest(Size.Zero);
            }

            if (double.IsInfinity(widthConstraint) && double.IsInfinity(heightConstraint))
            {
                return new SizeRequest(_svgSize.ToFormsSize());
            }

            var svgAspect = _svgSize.Width / _svgSize.Height;

            if (double.IsInfinity(widthConstraint))
            {
                return new SizeRequest(new Size(heightConstraint * svgAspect, heightConstraint));
            }

            if (double.IsInfinity(heightConstraint))
            {
                return new SizeRequest(new Size(widthConstraint, widthConstraint / svgAspect));
            }

            var constraintAspect = widthConstraint / heightConstraint;

            if (constraintAspect > svgAspect)
            {
                return new SizeRequest(new Size(heightConstraint * svgAspect, heightConstraint));
            }
            else
            {
                return new SizeRequest(new Size(widthConstraint, widthConstraint / svgAspect));
            }
        }

        private void PaintBackground(SKImageInfo info, SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear();

            var picture = LoadSvg();

            var targetSize = info.Rect.AspectFill(_svgSize.ToSizeI());
            var scale = targetSize.Size.Width / _svgSize.Width;

            var matrix = SKMatrix.MakeScale(scale, scale);
            canvas.DrawPicture(picture, ref matrix);
        }

        private SKPicture LoadSvg()
        {
            var svg = new SKSvg();
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_resourceName);
            var picture = svg.Load(stream);
            return picture;
        }
    }
}
