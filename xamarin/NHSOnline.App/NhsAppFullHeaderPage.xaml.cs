using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PageContent))]
    public partial class NhsAppFullHeaderPage : ContentPage
    {
        public event EventHandler<EventArgs>? SettingsRequested;
        public event EventHandler<EventArgs>? HelpRequested;
        public event EventHandler<EventArgs>? HomeRequested;
        public event EventHandler<EventArgs>? SymptomsRequested;
        public event EventHandler<EventArgs>? AppointmentsRequested;
        public event EventHandler<EventArgs>? PrescriptionsRequested;
        public event EventHandler<EventArgs>? RecordRequested;
        public event EventHandler<EventArgs>? MoreRequested;

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

        public ICommand SettingsClicked => new Command( () => SettingsRequested?.Invoke(this, EventArgs.Empty));

        public ICommand HelpClicked => new Command( () => HelpRequested?.Invoke(this, EventArgs.Empty));

        public ICommand SymptomsClicked => new Command(() => SymptomsRequested?.Invoke(this, EventArgs.Empty));

        public ICommand AppointmentsClicked => new Command( () => AppointmentsRequested?.Invoke(this, EventArgs.Empty));

        public ICommand PrescriptionsClicked => new Command( () => PrescriptionsRequested?.Invoke(this, EventArgs.Empty));

        public ICommand RecordClicked => new Command( () => RecordRequested?.Invoke(this, EventArgs.Empty));

        public ICommand MoreClicked => new Command( () => MoreRequested?.Invoke(this, EventArgs.Empty));

        public ICommand HomeClicked => new Command( () => HomeRequested?.Invoke(this, EventArgs.Empty));
    }
}
