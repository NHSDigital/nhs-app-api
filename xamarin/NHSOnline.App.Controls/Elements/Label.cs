using System.Linq;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Controls.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public sealed class Label : Xamarin.Forms.Label
    {
        private string _formattedTextContent = string.Empty;

        protected override void OnPropertyChanged(string propertyName = null!)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(FontSize):
                    FixFormattedTextFontSize();
                    break;
                case nameof(FormattedText):
                    if (HasFormattedTextContentChanged())
                    {
                        FixFormattedTextFontSize();
                    }
                    break;
                default:
                    break;
            }
        }

        // Workaround for Xamarin Issue: Setting font-family or font-weight breaks the font-size inheritance.
        // More details see https://github.com/xamarin/Xamarin.Forms/issues/2168
        private void FixFormattedTextFontSize()
        {
            if (FormattedText == null )
            {
                return;
            }

            foreach (var span in FormattedText.Spans)
            {
                span.FontSize = FontSize;
            }
        }

        private bool HasFormattedTextContentChanged()
        {
            var textContent = string.Join(string.Empty,FormattedText.Spans.Select(x => x.Text));

            if (Equals(_formattedTextContent, textContent))
            {
                return false;
            }

            _formattedTextContent = textContent;
            return true;
        }
    }
}