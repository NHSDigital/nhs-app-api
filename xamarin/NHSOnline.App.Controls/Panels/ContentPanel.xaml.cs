using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Panels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(PanelContent))]
    public partial class ContentPanel
    {
        public static readonly BindableProperty PanelContentProperty =
            BindableProperty.Create(nameof(PanelContent), typeof(View), typeof(ContentPanel));

        public ContentPanel()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveStates.SetVisualStateBreakpoints(this);
        }

        public View PanelContent
        {
            get => (View) GetValue(PanelContentProperty);
            set => SetValue(PanelContentProperty, value);
        }
    }
}