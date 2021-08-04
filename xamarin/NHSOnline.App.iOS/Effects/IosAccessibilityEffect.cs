using System;
using System.ComponentModel;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NHSOnline.App.iOS.Effects.IosAccessibilityEffect), nameof(AccessibilityEffect))]

namespace NHSOnline.App.iOS.Effects
{
    internal class IosAccessibilityEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if ((Control ?? Container) is { } target &&
                AccessibilityEffect.GetControlType(Element) is { } controlType)
            {
                AutomationProperties.SetIsInAccessibleTree(Element, true);
                target.AccessibilityTraits = AccessibilityTraitsFor(Element, controlType);
                target.IsAccessibilityElement = true;
            }

            if (Element is VisualElement visualElement)
            {
                visualElement.Focused -= VisualElementOnFocusStateChanged;
                visualElement.Focused += VisualElementOnFocusStateChanged;
                visualElement.Unfocused -= VisualElementOnFocusStateChanged;
                visualElement.Unfocused += VisualElementOnFocusStateChanged;
            }
        }

        private void VisualElementOnFocusStateChanged(object sender, FocusEventArgs e)
        {
            if (Element is VisualElement visualElement)
            {
                KeyboardFocusStates.SetKeyboardFocusState(visualElement, e.IsFocused);
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName.Equals(nameof(UIAccessibilityTrait.Selected), StringComparison.Ordinal))
            {
                if ((Control ?? Container) is { } target &&
                    AccessibilityEffect.GetControlType(Element) is { } controlType)
                {
                    target.AccessibilityTraits = AccessibilityTraitsFor(Element, controlType);
                }
            }
        }

        private static UIAccessibilityTrait AccessibilityTraitsFor(BindableObject element,
            AccessibilityEffect.ControlType controlType)
        {
            var traits = UIAccessibilityTrait.None;
            traits |= controlType switch
            {
                AccessibilityEffect.ControlType.Button => UIAccessibilityTrait.Button,
                AccessibilityEffect.ControlType.Link => UIAccessibilityTrait.Link,
                _ => throw new ArgumentOutOfRangeException(nameof(controlType), controlType,
                    $"{nameof(AccessibilityTraitsFor)} doesn't cover all types")
            };

            if (AccessibilityEffect.GetSelected(element))
            {
                traits |= UIAccessibilityTrait.Selected;
            }

            return traits;
        }

        protected override void OnDetached()
        {
            if (Element is VisualElement visualElement)
            {
                visualElement.Focused -= VisualElementOnFocusStateChanged;
                visualElement.Unfocused -= VisualElementOnFocusStateChanged;
            }
        }
    }
}