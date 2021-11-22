using System.Linq;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.Effects
{
    public static class AccessibilityEffect
    {
        public static readonly BindableProperty ControlTypeProperty = BindableProperty.CreateAttached(
            "ControlType", typeof(ControlType?), typeof(AccessibilityEffect), null, propertyChanged: OnControlTypeChanged);
        public static readonly BindableProperty SelectedProperty = BindableProperty.CreateAttached(
            "Selected", typeof(bool), typeof(AccessibilityEffect), false);

        public static ControlType? GetControlType(BindableObject view)
        {
            if (view.GetValue(ControlTypeProperty) is ControlType controlType)
            {
                return controlType;
            }

            return null;
        }

        public static void SetControlType(BindableObject view, ControlType? value)
        {
            view.SetValue(ControlTypeProperty, value);
        }

        public static bool GetSelected(BindableObject view)
        {
            if (view.GetValue(SelectedProperty) is bool selected)
            {
                return selected;
            }

            return false;
        }

        public static void SetSelected(BindableObject view, bool value)
        {
            view.SetValue(SelectedProperty, value);
        }

        static void OnControlTypeChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view)
            {
                var existingEffect = view.Effects.FirstOrDefault(e => e is AccessibilityRoutingEffect);

                if (newValue is ControlType)
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

        public enum ControlType
        {
            Link,
            Button,
            Heading1,
            Heading2,
            Spinner,
        }

        private class AccessibilityRoutingEffect : RoutingEffect
        {
            public AccessibilityRoutingEffect() : base($"{NhsAppEffects.ResolutionGroupName}.{nameof(AccessibilityEffect)}")
            {
            }
        }
    }
}