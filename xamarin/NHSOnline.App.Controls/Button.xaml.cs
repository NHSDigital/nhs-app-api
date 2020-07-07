using System;
using System.Windows.Input;
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
            BindableProperty.Create(nameof(ColourGradient), typeof(Color), typeof(Button));
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Button));

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

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public Button()
        {
            InitializeComponent();
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