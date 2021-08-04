using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NhsLoginButton
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(NhsLoginButton));

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public NhsLoginButton()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // these will be updated when we do the additional ticket
            if (ResponsiveStates.IsSmallScreen)
            {
                FocusedButton.HeightRequest = 48;
                UnfocusedButton.HeightRequest = 48;
            }
            else
            {
                FocusedButton.HeightRequest = 50;
                UnfocusedButton.HeightRequest = 50;
            }
        }
    }
}