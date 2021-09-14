using NHSOnline.App.Controls.Effects;
using NHSOnline.App.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(StackLayout), typeof(StackLayoutRenderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal sealed class StackLayoutRenderer : VisualElementRenderer<StackLayout>
    {
        public override bool CanBecomeFocused => CanLayoutBecomeFocused();

        public override void DidUpdateFocus(UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
        {
            if (CanBecomeFocused)
            {
                Element?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, Focused);
            }

            base.DidUpdateFocus(context, coordinator);
        }

        private bool CanLayoutBecomeFocused()
        {
            if (Element == null || AccessibilityEffect.GetControlType(Element) == null)
            {
                return false;
            }

            return AccessibilityEffect.GetControlType(Element) == AccessibilityEffect.ControlType.Button
                || AccessibilityEffect.GetControlType(Element) == AccessibilityEffect.ControlType.Link;
        }
    }
}