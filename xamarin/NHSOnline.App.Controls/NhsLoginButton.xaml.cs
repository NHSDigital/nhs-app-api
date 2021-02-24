using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NhsLoginButton
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(NhsLoginButton));

        public static readonly BindableProperty ButtonColourProperty =
            BindableProperty.Create(nameof(ButtonColour), typeof(Color), typeof(NhsLoginButton));

        public static readonly BindableProperty PressedButtonColourProperty =
            BindableProperty.Create(nameof(PressedButtonColour), typeof(Color), typeof(NhsLoginButton));

        public static readonly BindableProperty TextColourProperty =
            BindableProperty.Create(nameof(TextColour), typeof(Color), typeof(NhsLoginButton));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(NhsLoginButton));

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

        public Color PressedButtonColour
        {
            get => (Color) GetValue(PressedButtonColourProperty);
            set => SetValue(PressedButtonColourProperty, value);
        }

        public Color TextColour
        {
            get => (Color) GetValue(TextColourProperty);
            set => SetValue(TextColourProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public NhsLoginButton()
        {
            InitializeComponent();
        }
    }
}