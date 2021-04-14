using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace NHSOnline.App.Controls
{
    public class SvgImage : ContentView
    {
        private string? _resourceName;

        private SKSize _svgSize;

        public static readonly BindableProperty IconNameProperty =
            BindableProperty.Create(nameof(IconName), typeof(AppIcons), typeof(SvgImage));

        public SvgImage()
        {
            InitSvgImage();
        }
        internal SvgImage(string svgPath)
        {
            InitSvgImage(svgPath);
        }

        private void InitSvgImage(string? svgPath = null)
        {
            SetResourceName(svgPath);
            var svg = LoadSvg();
            var canvasView = new SKCanvasView();
            canvasView.PaintSurface += (sender, args) => PaintBackground(args.Info, args.Surface, svg.Picture);
            Content = canvasView;
            _svgSize = svg.Picture.CullRect.Size;
        }

        private void SetResourceName(string? svgPath)
        {
            if (svgPath != null)
            {
                _resourceName = $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{svgPath}";
            }
            else
            {
                _resourceName = IconName switch
                {
                    AppIcons.Home =>
                        $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{nameof(Icons)}.icon-home.svg",

                    AppIcons.Advice =>
                        $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{nameof(Icons)}.icon-symptoms.svg",

                    AppIcons.Appointments =>
                        $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{nameof(Icons)}.icon-appointments.svg",

                    AppIcons.Help =>
                        $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{nameof(Icons)}.icon-help.svg",

                    AppIcons.More =>
                        $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{nameof(Icons)}.icon-more.svg",

                    AppIcons.MyRecord =>
                        $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{nameof(Icons)}.icon-record.svg",

                    AppIcons.Prescriptions =>
                        $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}.{nameof(Icons)}.icon-prescriptions.svg",

                    _ => $"{nameof(NHSOnline)}.{nameof(App)}.{nameof(Controls)}"
                };
            }
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
            if (propertyName == "IconName")
            {
                InitSvgImage();
            }
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

        private void PaintBackground(SKImageInfo info, SKSurface surface, SKPicture picture)
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
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_resourceName);
            svg.Load(stream);
            return svg;
        }

        public AppIcons IconName
        {
            get => (AppIcons) GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }
    }
}
