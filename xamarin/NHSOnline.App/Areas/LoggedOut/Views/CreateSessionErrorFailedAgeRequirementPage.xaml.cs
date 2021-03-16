using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorFailedAgeRequirementPage : ICreateSessionErrorFailedAgeRequirementView, ICreateSessionErrorFailedAgeRequirementView.IEvents
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorFailedAgeRequirementPage), "3c");

        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionErrorFailedAgeRequirementView.IEvents> _appNavigation;

        public CreateSessionErrorFailedAgeRequirementPage(ILogger<CreateSessionErrorFailedAgeRequirementPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionErrorFailedAgeRequirementView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<ICreateSessionErrorFailedAgeRequirementView.IEvents> INavigationView<ICreateSessionErrorFailedAgeRequirementView.IEvents>.AppNavigation => _appNavigation;

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);
    }
}
