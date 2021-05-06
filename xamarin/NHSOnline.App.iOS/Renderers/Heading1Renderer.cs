using Foundation;
using NHSOnline.App.Controls;
using NHSOnline.App.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Heading1), typeof(Heading1Renderer))]
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

            if (e.NewElement is Heading1 newHeading)
            {
                newHeading.AccessibilityFocusChangeRequested += OnAccessibilityFocusChangeRequested;
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