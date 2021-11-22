using System;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NHSOnline.App.iOS.Effects.IosAccessibilityFocusEffect), nameof(AccessibilityFocusEffect))]
namespace NHSOnline.App.iOS.Effects
{
    public class IosAccessibilityFocusEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Element is IAccessibleControl focusResetControl)
            {
                focusResetControl.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
                focusResetControl.AccessibilityFocusChangeRequested += OnAccessibilityFocusChangeRequested;
            }
        }

        private void OnAccessibilityFocusChangeRequested(object sender, EventArgs e)
        {
            if (Element is not VisualElement view ||
                view.GetRenderer()?.NativeView == null)
            {
                return;
            }

            if ((Control ?? Container) is { } target)
            {
                UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, target);
            }
        }

        protected override void OnDetached()
        {
            if (Element is IAccessibleControl visibleControl)
            {
                visibleControl.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
            }
        }
    }
}