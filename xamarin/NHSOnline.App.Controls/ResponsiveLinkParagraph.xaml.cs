using System.Net.Mime;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResponsiveLinkParagraph
    {
        public static readonly BindableProperty LeadInTextProperty =
            BindableProperty.Create(nameof(LeadInText), typeof(string), typeof(ResponsiveLinkParagraph));

        public static readonly BindableProperty LinkTextProperty =
            BindableProperty.Create(nameof(LinkText), typeof(string), typeof(ResponsiveLinkParagraph));

        public static readonly BindableProperty LeadOutTextProperty =
            BindableProperty.Create(nameof(LeadOutText), typeof(string), typeof(ResponsiveLinkParagraph));

        public static readonly BindableProperty TappedProperty =
            BindableProperty.Create(nameof(Tapped), typeof(ICommand), typeof(ResponsiveLinkParagraph));

        public ResponsiveLinkParagraph()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var isWideDevice = Device.info.ScaledScreenSize.Width >= Breakpoints.WideScreenSize;

            VisualStateManager.GoToState(this, isWideDevice ? "Wide" : "Narrow");
        }

        public string LinkText
        {
            get => (string)GetValue(LinkTextProperty);
            set => SetValue(LinkTextProperty, value);
        }

        public string LeadInText
        {
            get => (string)GetValue(LeadInTextProperty);
            set => SetValue(LeadInTextProperty, value);
        }

        public string LeadOutText
        {
            get => (string)GetValue(LeadOutTextProperty);
            set => SetValue(LeadOutTextProperty, value);
        }

        public ICommand Tapped
        {
            get => (ICommand)GetValue(TappedProperty);
            set => SetValue(TappedProperty, value);
        }
    }
}
