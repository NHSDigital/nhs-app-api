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

        public FullNavigationFooterWide()
        {
            InitializeComponent();
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
    }
}
