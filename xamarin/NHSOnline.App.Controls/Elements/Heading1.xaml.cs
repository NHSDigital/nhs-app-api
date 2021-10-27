using System;
using NHSOnline.App.Controls.Styles;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Heading1
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Heading1));

        public static readonly BindableProperty TextColourProperty =
            BindableProperty.Create(nameof(TextColour), typeof(Color), typeof(Heading1), NhsUkColours.NhsUkPrimaryText);

        public event EventHandler<FocusRequestArgs>? AccessibilityFocusChangeRequested;

        public Heading1()
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

        public Color TextColour
        {
            get => (Color) GetValue(TextColourProperty);
            set => SetValue(TextColourProperty, value);
        }

        public void AccessibilityFocus()
        {
            if (AccessibilityFocusChangeRequested != null)
            {
                var arg = new FocusRequestArgs {Focus = true};
                AccessibilityFocusChangeRequested(this, arg);
            }
        }
    }
}