using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSAlert
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        private IOSAlert(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static IOSAlert WithText(IIOSInteractor interactor, string text) => new(interactor, text);

        public void AssertVisible() => _interactor.ActOnDriver(driver => driver.SwitchTo().Alert().Text.Should().Be(_text));

        public void Accept() => _interactor.ActOnDriver(driver => driver.SwitchTo().Alert().Accept());
    }
}
