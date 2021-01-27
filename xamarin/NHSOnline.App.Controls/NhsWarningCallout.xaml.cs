using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PanelContent))]
    public partial class NhsWarningCallout
    {
        public static readonly BindableProperty PanelContentProperty =
            BindableProperty.Create(nameof(PanelContent), typeof(View), typeof(NhsWarningCallout));

        public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(NhsWarningCallout));

        public NhsWarningCallout()
        {
            InitializeComponent();
        }

        public string LabelText
        {
            get => (string) GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public View PanelContent
        {
            get => (View) GetValue(PanelContentProperty);
            set => SetValue(PanelContentProperty, value);
        }
    }
}