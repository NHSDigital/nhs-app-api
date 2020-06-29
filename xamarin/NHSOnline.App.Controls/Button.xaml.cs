using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Button
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Button));
        public event EventHandler Clicked = null!;
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public Button()
        {
            InitializeComponent();
        }
        private void EncapsulatedButton_OnClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
        private void EncapsulatedButton_OnPressed(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed");
        }
        private void EncapsulatedButton_OnReleased(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal");
        }
    }
}