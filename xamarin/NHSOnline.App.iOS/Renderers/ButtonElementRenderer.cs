using NHSOnline.App.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(ButtonElementRenderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal sealed class ButtonElementRenderer : ButtonRenderer
    {
        public override bool CanBecomeFocused => true;

        public override void DidUpdateFocus(UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
        {
            base.DidUpdateFocus(context, coordinator);
            Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, Focused);
        }
    }
}