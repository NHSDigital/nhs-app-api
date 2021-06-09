using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResponsiveHeading1
    {
        public static readonly BindableProperty CommandProperty = TapGestureRecognizer.CommandProperty;

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Paragraph));

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(FormattedString), typeof(Paragraph), default(FormattedString));

        public ResponsiveHeading1()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var isWideDevice = Device.info.ScaledScreenSize.Width >= Breakpoints.WideScreenSize;

            VisualStateManager.GoToState(this, isWideDevice ? "Wide" : "Narrow");
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public FormattedString TextColor
        {
            get => (FormattedString)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
    }
}