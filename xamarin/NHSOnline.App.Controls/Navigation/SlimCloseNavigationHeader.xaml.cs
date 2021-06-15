using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlimCloseNavigationHeader
    {
        public static readonly BindableProperty CloseCommandProperty =
            BindableProperty.Create(nameof(CloseCommand), typeof(ICommand), typeof(SlimCloseAndLogoNavigationHeader));

        public SlimCloseNavigationHeader()
        {
            InitializeComponent();
        }

        public ICommand CloseCommand
        {
            get => (ICommand) GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }
    }
}
