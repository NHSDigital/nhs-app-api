using System.ComponentModel;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionPage: ICreateSessionView
    {
        private readonly IAppNavigation<ICreateSessionView> _appNavigation;

        public CreateSessionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _appNavigation = new AppNavigation<ICreateSessionView>(this, Navigation);
        }

        IAppNavigation<ICreateSessionView> INavigationView<ICreateSessionView>.AppNavigation => _appNavigation;
    }
}
