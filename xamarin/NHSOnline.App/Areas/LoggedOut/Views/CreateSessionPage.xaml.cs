using System.ComponentModel;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionPage: ICreateSessionView, ICreateSessionView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionView.IEvents> _appNavigation;

        public CreateSessionPage(ILogger<CreateSessionPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionView.IEvents>(this, Navigation);

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<ICreateSessionView.IEvents> INavigationView<ICreateSessionView.IEvents>.AppNavigation => _appNavigation;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }
    }
}
