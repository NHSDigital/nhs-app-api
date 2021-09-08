using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Navigation.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppTransparentHeaderPage
    {
        public static readonly BindableProperty PageContentProperty =
            BindableProperty.Create(nameof(PageContent), typeof(View), typeof(NhsAppFullHeaderPage));

        public NhsAppTransparentHeaderPage()
        {
            InitializeComponent();
        }

        public View PageContent
        {
            get => (View) GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }
    }
}
