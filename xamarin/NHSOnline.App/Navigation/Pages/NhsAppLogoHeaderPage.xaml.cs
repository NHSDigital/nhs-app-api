using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Navigation.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppLogoHeaderPage
    {
        public static readonly BindableProperty PageContentProperty =
            BindableProperty.Create(nameof(PageContent), typeof(View), typeof(NhsAppLogoHeaderPage));

        public NhsAppLogoHeaderPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public View PageContent
        {
            get => (View) GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }
    }
}
