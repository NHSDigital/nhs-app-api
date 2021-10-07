using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class AccessibleStackLayout: StackLayout
    {
        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(AccessibleStackLayout), false);

        public static readonly BindableProperty IsKeyboardFocusedProperty =
            BindableProperty.Create(nameof(IsKeyboardFocused), typeof(bool), typeof(AccessibleStackLayout), false);

        protected override void OnPropertyChanged(string propertyName = null!)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsSelected):
                case nameof(IsKeyboardFocused):
                    SetVisualStates();
                    break;
                default:
                    break;
            }
        }

        private void SetVisualStates()
        {
            KeyboardFocusStates.SetSelectedIconKeyboardFocusState(this, IsSelected, IsKeyboardFocused);
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public bool IsKeyboardFocused
        {
            get => (bool) GetValue(IsKeyboardFocusedProperty);
            set => SetValue(IsKeyboardFocusedProperty, value);
        }
    }
}