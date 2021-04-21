using System.Linq;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.Effects
{
    public static class FixedFontSizeEffect
    {
        public static readonly BindableProperty HasFixedFontSizeProperty =
            BindableProperty.CreateAttached("HasFixedFontSize", typeof(bool),
                typeof(FixedFontSizeEffect), false,
                propertyChanged: OnHasFixedFontSizeChanged);

        public static bool GetHasFixedFontSize(BindableObject view)
        {
            return (bool)view.GetValue(HasFixedFontSizeProperty);
        }

        public static void SetHasFixedFontSize(BindableObject view, bool value)
        {
            view.SetValue(HasFixedFontSizeProperty, value);
        }

        static void OnHasFixedFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view)
            {
                var hasEffect = (bool)newValue;
                if (hasEffect)
                {
                    view.Effects.Add(new FixedFontSizeRoutingEffect());
                }
                else
                {
                    var toRemove = view.Effects.FirstOrDefault(e => e is FixedFontSizeRoutingEffect);
                    if (toRemove != null)
                    {
                        view.Effects.Remove(toRemove);
                    }
                }
            }
        }

        class FixedFontSizeRoutingEffect : RoutingEffect
        {
            public FixedFontSizeRoutingEffect() : base($"{NhsAppEffects.ResolutionGroupName}.{nameof(FixedFontSizeEffect)}")
            {
            }
        }
    }
}