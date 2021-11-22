using Android.Views.Accessibility;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Action = Android.Views.Accessibility.Action;

[assembly: ExportEffect(typeof(NHSOnline.App.Droid.Effects.AndroidAccessibilityFocusEffect), nameof(AccessibilityFocusEffect))]

namespace NHSOnline.App.Droid.Effects
{
    internal sealed class AndroidAccessibilityFocusEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Element is IAccessibleControl accessibleControl)
            {
                accessibleControl.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
                accessibleControl.AccessibilityFocusChangeRequested += OnAccessibilityFocusChangeRequested;
            }
        }

        private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
        {
            if ((Control ?? Container) is { } target)
            {
                target.PerformAccessibilityAction(Action.AccessibilityFocus, null);
                target.SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
            }
        }

        protected override void OnDetached()
        {
            if (Element is IAccessibleControl accessibleControl)
            {
                accessibleControl.AccessibilityFocusChangeRequested -= OnAccessibilityFocusChangeRequested;
            }
        }
    }
}