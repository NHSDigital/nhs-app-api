using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PanelContent))]
    public partial class NhsPanelWithLabel
    {
        public static readonly BindableProperty PanelContentProperty =
            BindableProperty.Create(nameof(PanelContent), typeof(View), typeof(NhsPanelWithLabel));
        public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(NhsPanelWithLabel));

        public static readonly BindableProperty LabelBackgroundColourProperty = BindableProperty.Create(
            nameof(LabelBackgroundColour), typeof(Color), typeof(NhsPanelWithLabel),
            NhsUkColours.NhsUkBlue);

        public static readonly BindableProperty LabelTextColourProperty = BindableProperty.Create(
            nameof(LabelTextColour), typeof(Color), typeof(NhsPanelWithLabel),
            NhsUkColours.NhsUkWhite);

        public static readonly BindableProperty PanelBackgroundColourProperty =
            BindableProperty.Create(nameof(PanelBackgroundColour), typeof(Color), typeof(NhsPanelWithLabel),
                NhsUkColours.NhsUkWhite);

        public static readonly BindableProperty PanelHighlightColourProperty =
            BindableProperty.Create(nameof(PanelHighlightColour), typeof(Color), typeof(NhsPanelWithLabel),
                Color.Transparent);


        public NhsPanelWithLabel()
        {
            InitializeComponent();
        }

        public View PanelContent
        {
            get => (View)GetValue(PanelContentProperty);
            set => SetValue(PanelContentProperty, value);
        }

        public string LabelText
        {
            get => (string) GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public Color LabelBackgroundColour
        {
            get => (Color)GetValue(LabelBackgroundColourProperty);
            set => SetValue(LabelBackgroundColourProperty, value);
        }

        public Color LabelTextColour
        {
            get => (Color) GetValue(LabelTextColourProperty);
            set => SetValue(LabelTextColourProperty, value);
        }

        public Color PanelBackgroundColour
        {
            get => (Color) GetValue(PanelBackgroundColourProperty);
            set => SetValue(PanelBackgroundColourProperty, value);
        }

        public Color PanelHighlightColour
        {
            get => (Color) GetValue(PanelHighlightColourProperty);
            set => SetValue(PanelHighlightColourProperty, value);
        }
    }
}