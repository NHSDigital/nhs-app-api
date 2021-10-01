using Android.Content;
using Android.Views.Accessibility;
using NHSOnline.App.Controls.Elements;
using NHSOnline.App.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Spinner), typeof(SpinnerRenderer))]
namespace NHSOnline.App.Droid.Renderers
{
    internal sealed class SpinnerRenderer : ViewRenderer<ContentView, Android.Views.View>
    {
        public SpinnerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is Spinner oldSpinner)
            {
                oldSpinner.AccessibilityFocusChangeRequested -= AccessibilityFocusChangeRequested;
            }

            if (e.NewElement is Spinner newSpinner)
            {
                newSpinner.AccessibilityFocusChangeRequested += AccessibilityFocusChangeRequested;
            }
        }

        private void AccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
        {
            var spinnerView  = Element.Content.GetRenderer()?.View;
            spinnerView?.PerformAccessibilityAction(Action.AccessibilityFocus, null);
            SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
        }
    }
}