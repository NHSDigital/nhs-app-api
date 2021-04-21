using System.Linq;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.Effects
{
    public static class AccessibilityEffect
    {
        public static readonly BindableProperty AccessibilityControlTypeProperty =
            BindableProperty.CreateAttached(nameof(AccessibilityControlType), typeof(AccessibilityControlType?),
                typeof(AccessibilityEffect), null, propertyChanged: OnAccessibilityControlTypeChanged);

        public static AccessibilityControlType? GetAccessibilityControlType (BindableObject view)
        {
            if (view.GetValue(AccessibilityControlTypeProperty) is AccessibilityControlType controlType)
            {
                return controlType;
            }

            return null;
        }

        public static void SetAccessibilityControlType(BindableObject view, AccessibilityControlType? value)
        {
            view.SetValue(AccessibilityControlTypeProperty, value);
        }

        static void OnAccessibilityControlTypeChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view)
            {
                var existingEffect = view.Effects.FirstOrDefault(e => e is AccessibilityRoutingEffect);

                if (newValue is AccessibilityControlType)
                {
                    if (existingEffect is null)
                    {
                        view.Effects.Add(new AccessibilityRoutingEffect());
                    }
                }
                else
                {
                    if (existingEffect != null)
                    {
                        view.Effects.Remove(existingEffect);
                    }
                }
            }
        }

        public enum AccessibilityControlType
        {
            Link,
            Button
        }

        class AccessibilityRoutingEffect : RoutingEffect
        {
            public AccessibilityRoutingEffect() : base($"{NhsAppEffects.ResolutionGroupName}.{nameof(AccessibilityEffect)}")
            {
            }
        }
    }
}