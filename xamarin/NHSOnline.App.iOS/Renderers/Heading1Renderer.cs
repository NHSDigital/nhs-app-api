using Foundation;
using NHSOnline.App.Controls.Elements;
using NHSOnline.App.Controls.Elements.Deprecated;
using NHSOnline.App.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Heading1), typeof(Heading1Renderer))]
[assembly: ExportRenderer(typeof(ResponsiveHeading1), typeof(Heading1Renderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal sealed class Heading1Renderer : ViewRenderer<ContentView, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is Heading1 oldHeading)
            {
                oldHeading.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
            }
            else if (e.OldElement is ResponsiveHeading1 oldResponsiveHeading)
            {
                oldResponsiveHeading.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
            }

            if (e.NewElement is Heading1 newHeading)
            {
                newHeading.AccessibilityFocusChangeRequested += OnAccessibilityFocusChangeRequested;
            }
            else if (e.NewElement is ResponsiveHeading1 newResponsiveHeading)
            {
                newResponsiveHeading.AccessibilityFocusChangeRequested += OnAccessibilityFocusChangeRequested;
            }
        }

        private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
        {
            if (Element.Content.GetRenderer()?.NativeView is NSObject uiView)
            {
                UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, uiView);
            }
        }
    }
}