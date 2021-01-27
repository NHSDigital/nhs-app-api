using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public class GradientBackground : ContentView
    {
        public enum RadiusMode
        {
            Max,
            Min
        }

        private static readonly BindableProperty InnerColourProperty =
            BindableProperty.Create(nameof(InnerColour), typeof(Color), typeof(GradientBackground));

        private static readonly BindableProperty OuterColourProperty =
            BindableProperty.Create(nameof(OuterColour), typeof(Color), typeof(GradientBackground));

        private static readonly BindableProperty RadiusModeProperty =
            BindableProperty.Create(nameof(Radius), typeof(RadiusMode), typeof(GradientBackground), defaultValue: RadiusMode.Min);

        public Color InnerColour
        {
            get => (Color) GetValue(InnerColourProperty);
            set => SetValue(InnerColourProperty, value);
        }

        public Color OuterColour
        {
            get => (Color) GetValue(OuterColourProperty);
            set => SetValue(OuterColourProperty, value);
        }

        public RadiusMode Radius
        {
            get => (RadiusMode) GetValue(RadiusModeProperty);
            set => SetValue(RadiusModeProperty, value);
        }

        public GradientBackground()
        {
            var canvasView = new SKCanvasView();
            canvasView.PaintSurface += (sender, args) => PaintBackground(args.Info, args.Surface);
            Content = canvasView;
        }

        private void PaintBackground(SKImageInfo info, SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear();

            using (var paint = new SKPaint())
            {
                var radius = Radius == RadiusMode.Max
                    ? Math.Max(info.Rect.Width, info.Rect.Height)
                    : Math.Min(info.Rect.Width, info.Rect.Height);

                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(info.Rect.MidX, info.Rect.MidY),
                    radius,
                    new[] {InnerColour.ToSKColor(), OuterColour.ToSKColor()},
                    null,
                    SKShaderTileMode.Clamp);

                canvas.DrawRect(info.Rect, paint);
            }
        }
    }
}