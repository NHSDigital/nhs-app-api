using System;
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

        public static readonly BindableProperty HighlightedIconProperty =
            BindableProperty.Create(nameof(HighlightedIcon), typeof(SvgImage), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty IconCommandProperty =
            BindableProperty.Create(nameof(IconCommand), typeof(ICommand), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FullNavigationFooterItem));

        public static readonly BindableProperty HighlightedProperty =
            BindableProperty.Create(nameof(Highlighted), typeof(bool), typeof(FullNavigationFooterItem));

        public FullNavigationFooterItem()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Highlighted):
                case nameof(DefaultIcon):
                case nameof(HighlightedIcon):
                    SetIcon();
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

        public SvgImage HighlightedIcon
        {
            get => (SvgImage) GetValue(HighlightedIconProperty);
            set => SetValue(HighlightedIconProperty, value);
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

        public bool Highlighted
        {
            get => (bool) GetValue(HighlightedProperty);
            set => SetValue(HighlightedProperty, value);
        }

        private void SetIcon() => Icon = Highlighted && IsSet(HighlightedIconProperty) ? HighlightedIcon : DefaultIcon;
    }
}
