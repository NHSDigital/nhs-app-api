using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(Icon))]
    public partial class FullNavigationFooterItem
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(SvgImage), typeof(FullNavigationFooterItem));

        public FullNavigationFooterItem()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public SvgImage Icon
        {
            get => (SvgImage) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
