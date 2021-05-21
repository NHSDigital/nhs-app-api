using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationFooterWide
    {
        private const int MediumScreenSize = 700;

        public static readonly BindableProperty AdviceCommandProperty =
            BindableProperty.Create(nameof(AdviceCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty AppointmentsCommandProperty =
            BindableProperty.Create(nameof(AppointmentsCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty PrescriptionsCommandProperty =
            BindableProperty.Create(nameof(PrescriptionsCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty YourHealthCommandProperty =
            BindableProperty.Create(nameof(YourHealthCommand), typeof(ICommand), typeof(FullNavigationFooter));
        public static readonly BindableProperty MessagesCommandProperty =
            BindableProperty.Create(nameof(MessagesCommand), typeof(ICommand), typeof(FullNavigationFooter));

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(NavigationFooterItem), typeof(FullNavigationFooter));
        public static readonly BindableProperty AppointmentsSelectedProperty =
            BindableProperty.Create(nameof(AppointmentsSelected), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty AdviceSelectedProperty =
            BindableProperty.Create(nameof(AdviceSelected), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty PrescriptionsSelectedProperty =
            BindableProperty.Create(nameof(PrescriptionsSelected), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty YourHealthSelectedProperty =
            BindableProperty.Create(nameof(YourHealthSelected), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty MessagesSelectedProperty =
            BindableProperty.Create(nameof(MessagesSelected), typeof(bool), typeof(FullNavigationFooter));

        public FullNavigationFooterWide()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(SelectedItem):
                    AdviceSelected = SelectedItem == NavigationFooterItem.Advice;
                    AppointmentsSelected = SelectedItem == NavigationFooterItem.Appointments;
                    PrescriptionsSelected = SelectedItem == NavigationFooterItem.Prescriptions;
                    YourHealthSelected = SelectedItem == NavigationFooterItem.YourHealth;
                    MessagesSelected = SelectedItem == NavigationFooterItem.Messages;
                    break;
                default:
                    break;
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            FooterGrid.Margin = Device.info.PixelScreenSize.Width >= MediumScreenSize ? new Thickness(4, 0) : new Thickness(0);
        }

        public ICommand AdviceCommand
        {
            get => (ICommand) GetValue(AdviceCommandProperty);
            set => SetValue(AdviceCommandProperty, value);
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

        public ICommand YourHealthCommand
        {
            get => (ICommand) GetValue(YourHealthCommandProperty);
            set => SetValue(YourHealthCommandProperty, value);
        }

        public ICommand MessagesCommand
        {
            get => (ICommand) GetValue(MessagesCommandProperty);
            set => SetValue(MessagesCommandProperty, value);
        }

        public NavigationFooterItem SelectedItem
        {
            get => (NavigationFooterItem) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public bool AdviceSelected
        {
            get => (bool) GetValue(AdviceSelectedProperty);
            set => SetValue(AdviceSelectedProperty, value);
        }

        public bool AppointmentsSelected
        {
            get => (bool) GetValue(AppointmentsSelectedProperty);
            set => SetValue(AppointmentsSelectedProperty, value);
        }

        public bool PrescriptionsSelected
        {
            get => (bool) GetValue(PrescriptionsSelectedProperty);
            set => SetValue(PrescriptionsSelectedProperty, value);
        }

        public bool YourHealthSelected
        {
            get => (bool) GetValue(YourHealthSelectedProperty);
            set => SetValue(YourHealthSelectedProperty, value);
        }

        public bool MessagesSelected
        {
            get => (bool) GetValue(MessagesSelectedProperty);
            set => SetValue(MessagesSelectedProperty, value);
        }
    }
}
