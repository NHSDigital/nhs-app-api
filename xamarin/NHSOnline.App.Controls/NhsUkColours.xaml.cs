using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NhsUkColours
    {
        public NhsUkColours()
        {
            InitializeComponent();
        }

        public static Color NhsUkBlue => GetColour();
        public static Color NhsUkWhite => GetColour();
        public static Color NhsUkPrimaryText => GetColour();

        private static Color GetColour([CallerMemberName] string name = "") => (Color) new NhsUkColours()[name];
    }
}