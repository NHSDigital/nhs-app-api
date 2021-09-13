using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorCodeParagraph
    {
        // Unhappy/partial workaround: Setting font-family or font-weight breaks the font-size inheritance.
        // More details see https://github.com/xamarin/Xamarin.Forms/issues/2168

        public static readonly BindableProperty LeadInTextProperty =
            BindableProperty.Create(nameof(LeadInText), typeof(string), typeof(ErrorCodeParagraph));

        public static readonly BindableProperty BoldTextProperty =
            BindableProperty.Create(nameof(BoldText), typeof(string), typeof(ErrorCodeParagraph));

        public static readonly BindableProperty AccessibleBoldTextLabelProperty =
            BindableProperty.Create(nameof(AccessibleBoldTextLabelProperty), typeof(string), typeof(ErrorCodeParagraph));

        public static readonly BindableProperty LeadOutTextProperty =
            BindableProperty.Create(nameof(LeadOutText), typeof(string), typeof(ErrorCodeParagraph));

        public ErrorCodeParagraph()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveStates.SetVisualStateBreakpoints(this);
        }

        public string LeadInText
        {
            get => (string) GetValue(LeadInTextProperty);
            set => SetValue(LeadInTextProperty, value);
        }

        public string BoldText
        {
            get => (string) GetValue(BoldTextProperty);
            set => SetValue(BoldTextProperty, value);
        }

        public string AccessibleBoldTextLabel
        {
            get => (string) GetValue(AccessibleBoldTextLabelProperty);
            set => SetValue(AccessibleBoldTextLabelProperty, value);
        }

        public string LeadOutText
        {
            get => (string) GetValue(LeadOutTextProperty);
            set => SetValue(LeadOutTextProperty, value);
        }
    }
}