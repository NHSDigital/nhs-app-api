using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationHeader
    {
        public static readonly BindableProperty MoreCommandProperty =
            BindableProperty.Create(nameof(MoreCommand), typeof(ICommand), typeof(FullNavigationHeader));
        public static readonly BindableProperty HelpCommandProperty =
            BindableProperty.Create(nameof(HelpCommand), typeof(ICommand), typeof(FullNavigationHeader));
        public static readonly BindableProperty HomeCommandProperty =
            BindableProperty.Create(nameof(HomeCommand), typeof(ICommand), typeof(FullNavigationHeader));

        public ICommand MoreCommand
        {
            get => (ICommand) GetValue(MoreCommandProperty);
            set => SetValue(MoreCommandProperty, value);
        }
        public ICommand HelpCommand
        {
            get => (ICommand) GetValue(HelpCommandProperty);
            set => SetValue(HelpCommandProperty, value);
        }
        public ICommand HomeCommand
        {
            get => (ICommand) GetValue(HomeCommandProperty);
            set => SetValue(HomeCommandProperty, value);
        }
        public FullNavigationHeader()
        {
            InitializeComponent();
        }
    }
}
