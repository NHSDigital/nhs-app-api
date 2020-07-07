using System.Windows.Input;
using NHSOnline.App.Areas;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppBackSlimHeaderPage : ContentPage
    {

        public static readonly BindableProperty PageContentProperty =
            BindableProperty.Create(nameof(PageContent), typeof(View), typeof(NhsAppFullHeaderPage));

        public NhsAppBackSlimHeaderPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public View PageContent
        {
            get => (View) GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }

        public ICommand BackArrowClicked => new Command(async () => await Navigation.PopAsync().PreserveThreadContext());
    }
}
