using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(BodyContent))]
    public partial class NhsYellowWarningCard
    {

        public static readonly BindableProperty BodyContentProperty =
            BindableProperty.Create(nameof(BodyContent), typeof(View), typeof(NhsYellowWarningCard));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(NhsYellowWarningCard));

        public NhsYellowWarningCard()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public View BodyContent
        {
            get => (View) GetValue(BodyContentProperty);
            set => SetValue(BodyContentProperty, value);
        }
    }
}