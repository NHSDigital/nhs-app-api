using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NHSOnline.App.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Navigation.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppFullHeaderPage
    {
        public Func<Task>? SettingsRequested { get; set; }
        public Func<Task>? HelpRequested { get; set; }
        public Func<Task>? HomeRequested { get; set; }
        public Func<Task>? SymptomsRequested { get; set; }
        public Func<Task>? AppointmentsRequested { get; set; }
        public Func<Task>? PrescriptionsRequested { get; set; }
        public Func<Task>? RecordRequested { get; set; }
        public Func<Task>? MoreRequested { get; set; }

        public static readonly BindableProperty PageContentProperty =
            BindableProperty.Create(nameof(PageContent), typeof(View), typeof(NhsAppFullHeaderPage));

        public NhsAppFullHeaderPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public View PageContent
        {
            get => (View) GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }

        public ICommand SettingsClicked => new AsyncCommand(() => SettingsRequested);

        public ICommand HelpClicked => new AsyncCommand(() => HelpRequested);

        public ICommand SymptomsClicked => new AsyncCommand(() => SymptomsRequested);

        public ICommand AppointmentsClicked => new AsyncCommand(() => AppointmentsRequested);

        public ICommand PrescriptionsClicked => new AsyncCommand(() => PrescriptionsRequested);

        public ICommand RecordClicked => new AsyncCommand(() => RecordRequested);

        public ICommand MoreClicked => new AsyncCommand(() => MoreRequested);

        public ICommand HomeClicked => new AsyncCommand(() => HomeRequested);
    }
}
