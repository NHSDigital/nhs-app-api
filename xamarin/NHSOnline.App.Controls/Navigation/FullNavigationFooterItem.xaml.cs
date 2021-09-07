using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationFooterItem
    {
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(SvgImage), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty DefaultIconProperty =
            BindableProperty.Create(nameof(DefaultIcon), typeof(SvgImage), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty IsSelectedIconProperty =
            BindableProperty.Create(nameof(SelectedIcon), typeof(SvgImage), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty IconCommandProperty =
            BindableProperty.Create(nameof(IconCommand), typeof(ICommand), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty FocusedIconProperty =
            BindableProperty.Create(nameof(FocusedIcon), typeof(SvgImage), typeof(FullNavigationFooterItem));

        public FullNavigationFooterItem()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveFooterSizeStates.SetFooterWidthVisualState(IconButton);
        }

        public SvgImage Icon
        {
            get => (SvgImage) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public SvgImage DefaultIcon
        {
            get => (SvgImage) GetValue(DefaultIconProperty);
            set => SetValue(DefaultIconProperty, value);
        }

        public SvgImage SelectedIcon
        {
            get => (SvgImage) GetValue(IsSelectedIconProperty);
            set => SetValue(IsSelectedIconProperty, value);
        }

        public SvgImage FocusedIcon
        {
            get => (SvgImage) GetValue(FocusedIconProperty);
            set => SetValue(FocusedIconProperty, value);
        }

        public ICommand IconCommand
        {
            get => (ICommand) GetValue(IconCommandProperty);
            set => SetValue(IconCommandProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}
