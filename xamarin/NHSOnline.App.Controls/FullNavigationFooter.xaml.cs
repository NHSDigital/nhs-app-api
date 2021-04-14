using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationFooter
    {
        public static readonly BindableProperty SymptomsCommandProperty =
            BindableProperty.Create(nameof(SymptomsCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty AppointmentsCommandProperty =
            BindableProperty.Create(nameof(AppointmentsCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty PrescriptionsCommandProperty =
            BindableProperty.Create(nameof(PrescriptionsCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty RecordCommandProperty =
            BindableProperty.Create(nameof(RecordCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty MessagesCommandProperty =
            BindableProperty.Create(nameof(MessagesCommand), typeof(ICommand), typeof(FullNavigationFooter));

        public FullNavigationFooter()
        {
            InitializeComponent();
        }

        public ICommand SymptomsCommand
        {
            get => (ICommand) GetValue(SymptomsCommandProperty);
            set => SetValue(SymptomsCommandProperty, value);
        }

        public ICommand AppointmentsCommand
        {
            get => (ICommand) GetValue(AppointmentsCommandProperty);
            set => SetValue(AppointmentsCommandProperty, value);
        }

        public ICommand PrescriptionsCommand
        {
            get => (ICommand) GetValue(PrescriptionsCommandProperty);
            set => SetValue(PrescriptionsCommandProperty, value);
        }

        public ICommand RecordCommand
        {
            get => (ICommand) GetValue(RecordCommandProperty);
            set => SetValue(RecordCommandProperty, value);
        }

        public ICommand MessagesCommand
        {
            get => (ICommand) GetValue(MessagesCommandProperty);
            set => SetValue(MessagesCommandProperty, value);
        }
    }
}
