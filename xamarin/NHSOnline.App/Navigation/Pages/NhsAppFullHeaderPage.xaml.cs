using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Navigation.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppFullHeaderPage
    {
        private readonly IAccessibilityService _accessibilityService;
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(NhsAppFullHeaderPage));

        public static readonly BindableProperty PageContentProperty =
            BindableProperty.Create(nameof(PageContent), typeof(View), typeof(NhsAppFullHeaderPage));

        public static readonly BindableProperty SelectedNavigationFooterItemProperty =
            BindableProperty.Create(nameof(SelectedNavigationFooterItem), typeof(NavigationFooterItem), typeof(NhsAppFullHeaderPage));

        public static readonly BindableProperty PageDescriptionProperty =
            BindableProperty.Create(nameof(PageDescription), typeof(string), typeof(NhsAppCloseSlimHeaderPage), "");
        
        public NhsAppFullHeaderPage(IAccessibilityService accessibilityService)
        {
            _accessibilityService = accessibilityService;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public View PageContent
        {
            get => (View) GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }
        
        public string PageDescription
        {
            get => (string) GetValue(PageDescriptionProperty);
            set => SetValue(PageDescriptionProperty, value);
        }

        public NavigationFooterItem SelectedNavigationFooterItem
        {
            get => (NavigationFooterItem) GetValue(SelectedNavigationFooterItemProperty);
            set => SetValue(SelectedNavigationFooterItemProperty, value);
        }

        public Func<Task>? HelpRequested { get; set; }
        public ICommand HelpClicked => new AsyncCommand(() => HelpRequested);

        public Func<Task>? HomeRequested { get; set; }
        public ICommand HomeClicked => new AsyncCommand(() => HomeRequested);

        public Func<Task>? AdviceRequested { get; set; }
        public ICommand AdviceClicked => new AsyncCommand(() => AdviceRequested);

        public Func<Task>? AppointmentsRequested { get; set; }
        public ICommand AppointmentsClicked => new AsyncCommand(() => AppointmentsRequested);

        public Func<Task>? PrescriptionsRequested { get; set; }
        public ICommand PrescriptionsClicked => new AsyncCommand(() => PrescriptionsRequested);

        public Func<Task>? YourHealthRequested { get; set; }
        public ICommand YourHealthClicked => new AsyncCommand(() => YourHealthRequested);

        public Func<Task>? MoreRequested { get; set; }
        public ICommand MoreClicked => new AsyncCommand(() => MoreRequested);

        public Func<Task>? MessagesRequested { get; set; }
        public ICommand MessagesClicked => new AsyncCommand(() => MessagesRequested);
        
        protected override void OnAppearing()
        {
            try
            {
                if (!string.IsNullOrEmpty(PageDescription))
                {
                    _accessibilityService.AnnounceText($"{PageDescription}.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to announce text to voice over");
            }

            base.OnAppearing();
        }
    }
}
