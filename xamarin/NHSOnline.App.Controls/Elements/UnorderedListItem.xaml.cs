using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(Text))]
    public partial class UnorderedListItem
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(UnorderedListItem));

        public UnorderedListItem()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveStates.SetVisualStateBreakpoints(this);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}