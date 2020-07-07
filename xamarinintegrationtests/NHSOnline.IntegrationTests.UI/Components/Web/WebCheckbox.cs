using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebCheckbox
    {
        private readonly IWebInteractor _driver;
        private readonly InputBy _by;

        public WebCheckbox(IWebInteractor driver, string label)
        {
            _driver = driver;
            _by = new InputBy("checkbox", label);
        }

        public void Click()
        {
            _driver.ActOnElement(_by.Label, e => e.Click());
        }
    }
}
