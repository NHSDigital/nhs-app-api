using Android.Content;
using Android.Views.Accessibility;
using NHSOnline.App.Controls;
using NHSOnline.App.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
namespace NHSOnline.App.Droid.Renderers
{
    internal sealed class ContentPageRenderer : PageRenderer
    {
        public ContentPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is IAccessibleControl oldPage)
            {
                oldPage.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
            }

            if (e.NewElement is IAccessibleControl newPage)
            {
                newPage.AccessibilityFocusChangeRequested += OnAccessibilityFocusChangeRequested;
            }
        }

        private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
        {
            PerformAccessibilityAction(Action.ClearAccessibilityFocus, null);
            SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
        }
    }
}