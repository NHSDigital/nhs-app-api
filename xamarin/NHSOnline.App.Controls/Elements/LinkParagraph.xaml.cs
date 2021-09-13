using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinkParagraph
    {
        public static readonly BindableProperty LeadInTextProperty =
            BindableProperty.Create(nameof(LeadInText), typeof(string), typeof(LinkParagraph));

        public static readonly BindableProperty LinkTextProperty =
            BindableProperty.Create(nameof(LinkText), typeof(string), typeof(LinkParagraph));

        public static readonly BindableProperty LeadOutTextProperty =
            BindableProperty.Create(nameof(LeadOutText), typeof(string), typeof(LinkParagraph));

        public static readonly BindableProperty TappedProperty =
            BindableProperty.Create(nameof(Tapped), typeof(ICommand), typeof(LinkParagraph));

        public LinkParagraph()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveStates.SetVisualStateBreakpoints(this);
        }

        public string LinkText
        {
            get => (string) GetValue(LinkTextProperty);
            set => SetValue(LinkTextProperty, value);
        }

        public string LeadInText
        {
            get => (string) GetValue(LeadInTextProperty);
            set => SetValue(LeadInTextProperty, value);
        }

        public string LeadOutText
        {
            get => (string) GetValue(LeadOutTextProperty);
            set => SetValue(LeadOutTextProperty, value);
        }

        public ICommand Tapped
        {
            get => (ICommand) GetValue(TappedProperty);
            set => SetValue(TappedProperty, value);
        }
    }
}