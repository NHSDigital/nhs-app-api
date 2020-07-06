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

        public static readonly BindableProperty ButtonColourProperty =
            BindableProperty.Create(nameof(ButtonColour), typeof(Color), typeof(Button));
        public static readonly BindableProperty TextColourProperty =
            BindableProperty.Create(nameof(TextColour), typeof(Color), typeof(Button));
        public static readonly BindableProperty ColourGradientProperty =
            BindableProperty.Create(nameof(ColourGradient), typeof(Color), typeof(BoxView));
        public event EventHandler Clicked = null!;
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public Color ButtonColour
        {
            get => (Color) GetValue(ButtonColourProperty);
            set => SetValue(ButtonColourProperty, value);
        }
        public Color TextColour
        {
            get => (Color) GetValue(TextColourProperty);
            set => SetValue(TextColourProperty, value);
        }
        public Color ColourGradient
        {
            get => (Color) GetValue(ColourGradientProperty);
            set => SetValue(ColourGradientProperty, value);
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