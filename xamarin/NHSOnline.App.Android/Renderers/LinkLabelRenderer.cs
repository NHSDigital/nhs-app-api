using Android.Content;
using NHSOnline.App.Controls.Elements;
using NHSOnline.App.Controls.Elements.Deprecated;
using NHSOnline.App.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LinkLabel), typeof(LinkLabelRenderer))]
[assembly: ExportRenderer(typeof(ResponsiveLinkLabel), typeof(LinkLabelRenderer))]
namespace NHSOnline.App.Droid.Renderers
{
    internal sealed class LinkLabelRenderer: LabelRenderer
    {
        public LinkLabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.KeyPress -= ControlOnKeyPress;
                Control.KeyPress += ControlOnKeyPress;
            }
        }

        private void ControlOnKeyPress(object sender, KeyEventArgs e)
        {
            e.Handled = false;
            if (!e.IsEnterKeyReleaseEvent())
            {
                return;
            }

            if (Element is LinkLabel linkLabel)
            {
                linkLabel.Command.Execute(null);
                e.Handled = true;
            }
            else if (Element is ResponsiveLinkLabel responsiveLinkLabel)
            {
                responsiveLinkLabel.Command.Execute(null);
                e.Handled = true;
            }
        }
    }
}