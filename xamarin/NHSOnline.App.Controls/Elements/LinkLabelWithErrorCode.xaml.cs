using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinkLabelWithErrorCode
    {
        public static readonly BindableProperty CommandProperty = TapGestureRecognizer.CommandProperty;

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(LinkLabelWithErrorCode));

        public static readonly BindableProperty ErrorCodeProperty =
            BindableProperty.Create(nameof(ErrorCode), typeof(string), typeof(LinkLabelWithErrorCode));

        public LinkLabelWithErrorCode()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveStates.SetVisualStateBreakpoints(this);
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string ErrorCode
        {
            get => (string) GetValue(ErrorCodeProperty);
            set => SetValue(ErrorCodeProperty, value);
        }
    }
}