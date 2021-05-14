using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace NHSOnline.App.Controls
{
    public class SvgImage : ContentView
    {
        private readonly string _svgResourcePath;

        private SKSize _svgSize;

        internal SvgImage(string svgResourcePath)
        {
            _svgResourcePath = svgResourcePath;

            DrawSvg();
        }

        private void DrawSvg()
        {
            var svg = LoadSvg();
            _svgSize = svg.Picture.CullRect.Size;
            AutomationId = svg.Description;

            var canvasView = new SKCanvasView();
            canvasView.PaintSurface += (sender, args) => PaintSurfaceRequested(args.Info, args.Surface, svg.Picture);
            Content = canvasView;
        }

        private void PaintSurfaceRequested(SKImageInfo info, SKSurface surface, SKPicture picture)
        {
            var canvas = surface.Canvas;
            canvas.Clear();

            var targetSize = info.Rect.AspectFill(_svgSize.ToSizeI());
            var scale = targetSize.Size.Width / _svgSize.Width;
            var matrix = SKMatrix.CreateScale(scale, scale);

            canvas.DrawPicture(picture, ref matrix);
        }

        private SKSvg LoadSvg()
        {
            var svg = new SKSvg();
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_svgResourcePath);
            svg.Load(stream);
            return svg;
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

            return new SizeRequest(new Size(widthConstraint, widthConstraint / svgAspect));
        }
    }
}
