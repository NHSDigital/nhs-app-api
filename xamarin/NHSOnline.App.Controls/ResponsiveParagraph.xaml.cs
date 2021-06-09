using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(FormattedText))]
    public partial class ResponsiveParagraph
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Paragraph));

        public static readonly BindableProperty FormattedTextProperty =
            BindableProperty.Create(nameof(FormattedText), typeof(FormattedString), typeof(Paragraph), default(FormattedString));

        public ResponsiveParagraph()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var isWideDevice = Device.info.ScaledScreenSize.Width >= Breakpoints.WideScreenSize;

            VisualStateManager.GoToState(this, isWideDevice ? "Wide" : "Narrow");
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public FormattedString FormattedText
        {
            get => (FormattedString)GetValue(FormattedTextProperty);
            set => SetValue(FormattedTextProperty, value);
        }
    }
}
