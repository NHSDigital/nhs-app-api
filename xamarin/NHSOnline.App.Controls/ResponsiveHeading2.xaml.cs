using NHSOnline.App.Controls.Styles;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResponsiveHeading2
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Heading1));

        public static readonly BindableProperty TextColourProperty =
            BindableProperty.Create(nameof(TextColour), typeof(Color), typeof(Heading2), NhsUkColours.NhsUkPrimaryText);

        public ResponsiveHeading2()
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
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public Color TextColour
        {
            get => (Color) GetValue(TextColourProperty);
            set => SetValue(TextColourProperty, value);
        }
    }
}