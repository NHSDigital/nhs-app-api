using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
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

        public FullNavigationFooterItem()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsSelected):
                case nameof(DefaultIcon):
                case nameof(SelectedIcon):
                    SetVisualState();
                    break;
                default:
                    break;
            }
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

        private void SetVisualState()
        {
            var state = IsSelected && IsSet(IsSelectedIconProperty) ?
                VisualStateManager.CommonStates.Selected : VisualStateManager.CommonStates.Normal;
            VisualStateManager.GoToState(this, state);
        }
    }
}
