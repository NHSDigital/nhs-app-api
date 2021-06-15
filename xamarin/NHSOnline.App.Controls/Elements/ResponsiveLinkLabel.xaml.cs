using System.Windows.Input;
using NHSOnline.App.Controls.Styles;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResponsiveLinkLabel
    {
        public static readonly BindableProperty CommandProperty = TapGestureRecognizer.CommandProperty;

        public static readonly BindableProperty LabelTextColourProperty =
            BindableProperty.Create(nameof(LabelTextColour), typeof(Color), typeof(ResponsiveLinkLabel), NhsUkColours.NhsUkPrimaryLinkBlue);

        public static readonly BindableProperty LabelTextDecorationProperty =
            BindableProperty.Create(nameof(LabelTextDecoration), typeof(TextDecorations), typeof(Label), TextDecorations.Underline);

        public ResponsiveLinkLabel()
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
    }
}