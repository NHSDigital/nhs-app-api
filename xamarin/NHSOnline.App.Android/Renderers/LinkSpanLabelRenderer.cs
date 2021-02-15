using System.Linq;
using Android.Content;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Droid.Renderers;
using NHSOnline.App.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Label), typeof(LinkSpanLabelRenderer))]
namespace NHSOnline.App.Droid.Renderers
{
    internal sealed class LinkSpanLabelRenderer: LabelRenderer
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(LinkSpanLabelRenderer));

        public LinkSpanLabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control is not null)
            {
                Control.KeyPress -= ControlOnKeyPress;
                Control.KeyPress += ControlOnKeyPress;
            }
        }

        private void ControlOnKeyPress(object sender, KeyEventArgs e)
        {
            e.Handled = false;

            if (e.IsEnterKeyReleaseEvent() &&
                Element.FormattedText.Spans.OfType<LinkSpan>().FirstOrDefault() is { } linkSpan)
            {
                linkSpan.Tapped.Execute(null);
                e.Handled = true;
            }
        }
    }
}