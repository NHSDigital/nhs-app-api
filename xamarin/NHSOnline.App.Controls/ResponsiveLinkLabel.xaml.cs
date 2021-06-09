using System.Windows.Input;
using NHSOnline.App.Controls.Styles;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResponsiveLinkLabel
    {
        public static readonly BindableProperty CommandProperty = TapGestureRecognizer.CommandProperty;

        public static readonly BindableProperty LabelTextColourProperty = BindableProperty.Create(
            nameof(LabelTextColour), typeof(Color), typeof(LinkLabel),
            NhsUkColours.NhsUkPrimaryLinkBlue);

        public static readonly BindableProperty LabelTextDecorationProperty = BindableProperty.Create(
            nameof(LabelTextDecoration), typeof(TextDecorations), typeof(Label),
            TextDecorations.Underline);

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Paragraph));

        public static readonly BindableProperty FormattedTextProperty =
            BindableProperty.Create(nameof(FormattedText), typeof(FormattedString), typeof(Paragraph), default(FormattedString));

        public ResponsiveLinkLabel()
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

        public Color LabelTextColour
        {
            get => (Color) GetValue(LabelTextColourProperty);
            set => SetValue(LabelTextColourProperty, value);
        }

        public TextDecorations LabelTextDecoration
        {
            get => (TextDecorations) GetValue(LabelTextDecorationProperty);
            set => SetValue(LabelTextDecorationProperty, value);
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