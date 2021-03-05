using System;
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
                AccessibilityEffect.GetAccessibilityControlType(Element) is { } controlType)
            {
                AutomationProperties.SetIsInAccessibleTree(Element, true);
                target.AccessibilityTraits = AccessibilityTraitsFor(controlType);
                target.IsAccessibilityElement = true;
            }
        }

        private static UIAccessibilityTrait AccessibilityTraitsFor(AccessibilityEffect.AccessibilityControlType controlType)
        {
            return controlType switch
            {
                AccessibilityEffect.AccessibilityControlType.Button => UIAccessibilityTrait.Button,
                AccessibilityEffect.AccessibilityControlType.Link => UIAccessibilityTrait.Link,
                _ => throw new ArgumentOutOfRangeException(nameof(controlType), controlType,
                    $"{nameof(AccessibilityTraitsFor)} doesn't cover all types")
            };
        }

        protected override void OnDetached()
        {
        }
    }
}