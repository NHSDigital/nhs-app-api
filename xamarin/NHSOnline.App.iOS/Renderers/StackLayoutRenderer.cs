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
        public override bool CanBecomeFocused => AccessibilityEffect.GetControlType(Element) == AccessibilityEffect.ControlType.Button;

        public override void DidUpdateFocus(UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
        {
            base.DidUpdateFocus(context, coordinator);
            Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, Focused);
        }
    }
}