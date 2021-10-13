using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Navigation.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppCloseAndLogoHeaderPage
    {
        private readonly IAccessibilityService _accessibilityService;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(NhsAppCloseAndLogoHeaderPage));

        public static readonly BindableProperty PageContentProperty =
            BindableProperty.Create(nameof(PageContent), typeof(View), typeof(NhsAppFullHeaderPage));

        public static readonly BindableProperty CloseCommandProperty =
            BindableProperty.Create(nameof(CloseCommand), typeof(AsyncCommand), typeof(NhsAppFullHeaderPage));

        public static readonly BindableProperty PageDescriptionProperty =
            BindableProperty.Create(nameof(PageDescription), typeof(string), typeof(NhsAppCloseSlimHeaderPage));

        public NhsAppCloseAndLogoHeaderPage(IAccessibilityService accessibilityService)
        {
            _accessibilityService = accessibilityService;
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

        public string PageDescription
        {
            get => (string) GetValue(PageDescriptionProperty);
            set => SetValue(PageDescriptionProperty, value);
        }

        protected override void OnAppearing()
        {
            try
            {
                _accessibilityService!.AnnounceText($"{PageDescription}.");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to announce text to voice over");
            }

            base.OnAppearing();
        }

        private async Task DefaultCloseAction() => await Navigation.PopAsync(false).PreserveThreadContext();
    }
}