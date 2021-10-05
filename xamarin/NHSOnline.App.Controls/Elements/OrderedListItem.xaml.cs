using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(Text))]
    public partial class OrderedListItem
    {
        public static readonly BindableProperty NumberProperty =
            BindableProperty.Create(nameof(Number), typeof(int), typeof(OrderedListItem));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(OrderedListItem));

        public static readonly BindableProperty FormattedTextProperty =
            BindableProperty.Create(nameof(FormattedText), typeof(FormattedString), typeof(Paragraph), default(FormattedString));

        public OrderedListItem()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveStates.SetVisualStateBreakpoints(this);
        }

        public int Number
        {
            get => (int) GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public FormattedString FormattedText
        {
            get => (FormattedString) GetValue(FormattedTextProperty);
            set => SetValue(FormattedTextProperty, value);
        }
    }
}