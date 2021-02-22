using Android.Content;
using NHSOnline.App.Controls;
using NHSOnline.App.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LinkLabel), typeof(LinkLabelRenderer))]
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
            if (e.IsEnterKeyReleaseEvent() &&
                Element is LinkLabel linkLabel)
            {
                linkLabel.Command.Execute(null);
                e.Handled = true;
            }
        }
    }
}