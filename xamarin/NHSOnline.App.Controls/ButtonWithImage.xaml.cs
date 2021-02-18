using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ButtonWithImage
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonWithImage));

        public static readonly BindableProperty ButtonColourProperty =
            BindableProperty.Create(nameof(ButtonColour), typeof(Color), typeof(ButtonWithImage));

        public static readonly BindableProperty TextColourProperty =
            BindableProperty.Create(nameof(TextColour), typeof(Color), typeof(ButtonWithImage));

        public static readonly BindableProperty ColourGradientProperty =
            BindableProperty.Create(nameof(ColourGradient), typeof(Color), typeof(ButtonWithImage));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ButtonWithImage));

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(ButtonWithImage));

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

        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public ButtonWithImage()
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