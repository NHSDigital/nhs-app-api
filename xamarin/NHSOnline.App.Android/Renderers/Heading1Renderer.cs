using Android.Content;
using Android.Views.Accessibility;
using NHSOnline.App.Controls;
using NHSOnline.App.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Heading1), typeof(Heading1Renderer))]
namespace NHSOnline.App.Droid.Renderers
{
    internal sealed class Heading1Renderer : ViewRenderer<ContentView, Android.Views.View>
    {
        public Heading1Renderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is Heading1 oldHeading)
            {
                oldHeading.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
            }

            if (e.NewElement is Heading1 newHeading)
            {
                newHeading.AccessibilityFocusChangeRequested += OnAccessibilityFocusChangeRequested;
            }
        }

        private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
        {
            var textView = Element.Content.GetRenderer()?.View;
            textView?.PerformAccessibilityAction(Action.AccessibilityFocus, null);
        }
    }
}