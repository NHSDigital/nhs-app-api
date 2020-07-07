using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Heading1
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Label));

        public static readonly BindableProperty TextColourProperty = BindableProperty.Create(
            nameof(TextColour), typeof(Color), typeof(NhsPanelWithLabel),
            NhsUkColours.NhsUkPrimaryText);

        public Heading1()
        {
            InitializeComponent();
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
    }
}