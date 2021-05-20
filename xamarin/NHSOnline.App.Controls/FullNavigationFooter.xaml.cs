using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationFooter
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

        public static readonly BindableProperty HighlightedItemProperty =
            BindableProperty.Create(nameof(HighlightedItem), typeof(NavigationFooterItem), typeof(FullNavigationFooter));
        public static readonly BindableProperty AppointmentsHighlightedProperty =
            BindableProperty.Create(nameof(AppointmentsHighlighted), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty AdviceHighlightedProperty =
            BindableProperty.Create(nameof(AdviceHighlighted), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty PrescriptionsHighlightedProperty =
            BindableProperty.Create(nameof(PrescriptionsHighlighted), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty YourHealthHighlightedProperty =
            BindableProperty.Create(nameof(YourHealthHighlighted), typeof(bool), typeof(FullNavigationFooter));
        public static readonly BindableProperty MessagesHighlightedProperty =
            BindableProperty.Create(nameof(MessagesHighlighted), typeof(bool), typeof(FullNavigationFooter));


        public FullNavigationFooter()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            FooterGrid.Margin = Device.info.PixelScreenSize.Width >= MediumScreenSize ? new Thickness(4, 0) : new Thickness(0);
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(HighlightedItem):
                    AdviceHighlighted = HighlightedItem == NavigationFooterItem.Advice;
                    AppointmentsHighlighted = HighlightedItem == NavigationFooterItem.Appointments;
                    PrescriptionsHighlighted = HighlightedItem == NavigationFooterItem.Prescriptions;
                    YourHealthHighlighted = HighlightedItem == NavigationFooterItem.YourHealth;
                    MessagesHighlighted = HighlightedItem == NavigationFooterItem.Messages;
                    break;
                default:
                    break;
            }
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

        public NavigationFooterItem HighlightedItem
        {
            get => (NavigationFooterItem) GetValue(HighlightedItemProperty);
            set => SetValue(HighlightedItemProperty, value);
        }

        public bool AdviceHighlighted
        {
            get => (bool) GetValue(AdviceHighlightedProperty);
            set => SetValue(AdviceHighlightedProperty, value);
        }

        public bool AppointmentsHighlighted
        {
            get => (bool) GetValue(AppointmentsHighlightedProperty);
            set => SetValue(AppointmentsHighlightedProperty, value);
        }

        public bool PrescriptionsHighlighted
        {
            get => (bool) GetValue(PrescriptionsHighlightedProperty);
            set => SetValue(PrescriptionsHighlightedProperty, value);
        }

        public bool YourHealthHighlighted
        {
            get => (bool) GetValue(YourHealthHighlightedProperty);
            set => SetValue(YourHealthHighlightedProperty, value);
        }

        public bool MessagesHighlighted
        {
            get => (bool) GetValue(MessagesHighlightedProperty);
            set => SetValue(MessagesHighlightedProperty, value);
        }
    }
}
