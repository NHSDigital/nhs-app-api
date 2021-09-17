using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Navigation.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppCloseAndLogoHeaderPage
    {
        public static readonly BindableProperty PageContentProperty =
            BindableProperty.Create(nameof(PageContent), typeof(View), typeof(NhsAppFullHeaderPage));

        public static readonly BindableProperty CloseCommandProperty =
            BindableProperty.Create(nameof(CloseCommand), typeof(AsyncCommand), typeof(NhsAppFullHeaderPage));

        public NhsAppCloseAndLogoHeaderPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            CloseCommand = new AsyncCommand(() => DefaultCloseAction);
        }

        public View PageContent
        {
            get => (View) GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }

        public AsyncCommand CloseCommand
        {
            get => (AsyncCommand) GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        private async Task DefaultCloseAction() => await Navigation.PopAsync(false).PreserveThreadContext();
    }
}