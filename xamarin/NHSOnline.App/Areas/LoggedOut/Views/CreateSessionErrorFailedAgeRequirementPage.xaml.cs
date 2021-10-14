using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorFailedAgeRequirementPage : ICreateSessionErrorFailedAgeRequirementView, ICreateSessionErrorFailedAgeRequirementView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionErrorFailedAgeRequirementView.IEvents> _appNavigation;

        public CreateSessionErrorFailedAgeRequirementPage(ILogger<CreateSessionErrorFailedAgeRequirementPage> logger, IAccessibilityService accessibilityService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionErrorFailedAgeRequirementView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            base.OnAppearing();
        }

        IAppNavigation<ICreateSessionErrorFailedAgeRequirementView.IEvents> INavigationView<ICreateSessionErrorFailedAgeRequirementView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(CreateSessionErrorFailedAgeRequirementPage));
            return Task.CompletedTask;
        }
    }
}
