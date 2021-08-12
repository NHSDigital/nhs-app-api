using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationHeaderItem
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FullNavigationHeaderItem));

        public static readonly BindableProperty DefaultIconProperty =
            BindableProperty.Create(nameof(DefaultIcon), typeof(SvgImage), typeof(FullNavigationHeaderItem));

        public static readonly BindableProperty KeyboardFocusIconProperty =
            BindableProperty.Create(nameof(KeyboardFocusIcon), typeof(SvgImage), typeof(FullNavigationHeaderItem));

        public static readonly BindableProperty IconCommandProperty =
            BindableProperty.Create(nameof(IconCommand), typeof(ICommand), typeof(FullNavigationHeaderItem));

        public FullNavigationHeaderItem()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public SvgImage DefaultIcon
        {
            get => (SvgImage) GetValue(DefaultIconProperty);
            set => SetValue(DefaultIconProperty, value);
        }

        public SvgImage KeyboardFocusIcon
        {
            get => (SvgImage) GetValue(KeyboardFocusIconProperty);
            set => SetValue(KeyboardFocusIconProperty, value);
        }

        public ICommand IconCommand
        {
            get => (ICommand) GetValue(IconCommandProperty);
            set => SetValue(IconCommandProperty, value);
        }
    }
}