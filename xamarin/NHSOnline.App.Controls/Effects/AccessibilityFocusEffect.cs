using System.Linq;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.Effects
{
    public static class AccessibilityFocusEffect
    {
        public static readonly BindableProperty CanFocusProperty = BindableProperty.CreateAttached(
            "CanFocus", typeof(bool), typeof(AccessibilityFocusEffect), false, propertyChanged: OnCanFocusChanged);

        public static bool GetCanFocus(BindableObject view)
        {
            if (view.GetValue(CanFocusProperty) is bool selected)
            {
                return selected;
            }

            return false;
        }

        public static void SetCanFocus(BindableObject view, bool value)
        {
            view.SetValue(CanFocusProperty, value);
        }

        static void OnCanFocusChanged(BindableObject bindable, object oldValue, object newValue)
        {

            if (bindable is VisualElement visualElement)
            {
                var existingEffect = visualElement.Effects.FirstOrDefault(e => e is AccessibilityFocusRoutingEffect);

                if (newValue is true)
                {
                    if (existingEffect is null)
                    {
                        visualElement.Effects.Add(new AccessibilityFocusRoutingEffect());
                    }
                }
                else
                {
                    if (existingEffect != null)
                    {
                        visualElement.Effects.Remove(existingEffect);
                    }
                }
            }
        }

        private class AccessibilityFocusRoutingEffect : RoutingEffect
        {
            public AccessibilityFocusRoutingEffect() : base($"{NhsAppEffects.ResolutionGroupName}.{nameof(AccessibilityFocusEffect)}")
            {
            }
        }
    }
}