
using NHSOnline.App.Controls;
using NHSOnline.App.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal sealed class ContentPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
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
            UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, this);
        }
    }
}