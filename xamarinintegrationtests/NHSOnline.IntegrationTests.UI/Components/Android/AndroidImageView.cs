using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidImageView: ILayoutChild
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _imageViewDescription;

        private AndroidImageView(IAndroidInteractor interactor, string imageViewDescription)
        {
            _interactor = interactor;
            _imageViewDescription = imageViewDescription;
        }

        public static AndroidImageView WithImageViewDescription(IAndroidInteractor interactor, string imageViewDescription)
            => new AndroidImageView(interactor, imageViewDescription);

        private By FindBy
            => By.XPath($"//{Xpath}");

        public string Xpath
             => $"android.widget.ImageView[normalize-space(@content-desc)={_imageViewDescription.QuoteXPathLiteral()}]";
    }
}
