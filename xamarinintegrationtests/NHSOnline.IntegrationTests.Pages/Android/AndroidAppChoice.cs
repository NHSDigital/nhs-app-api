using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidAppChoice
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _targetApp;
        private readonly string? _link;

        private string OptionalLinkText => _link != null ? $" {_link} links" : " links";

        private AndroidSystemLabel OpenWithAppChoiceText => AndroidSystemLabel.WhichMatches(_driver,
            $"Open({OptionalLinkText})? with");

        private AndroidSystemLabel OpenWithPreselectedAppText =>
            AndroidSystemLabel.WhichMatches(_driver, $"Open({OptionalLinkText})? with {_targetApp}");

        private AndroidAppChoiceOption TargetAppChoice => AndroidAppChoiceOption.WithText(_driver, _targetApp);

        private AndroidSystemButton JustOnceButton =>
            AndroidSystemButton.WhichMatches(_driver, "(JUST ONCE|Just once)");

        public AndroidAppChoice(IAndroidDriverWrapper driver, string targetApp, string? link = null)
        {
            _driver = driver;
            _targetApp = targetApp;
            _link = link;
        }

        public void AssertDisplayed()
        {
            IsDisplayed().Should().BeTrue("expected android app choice to be displayed");
        }

        public bool IsDisplayed() => OpenWithAppChoiceText.IsPresent() || OpenWithPreselectedAppText.IsPresent();

        public void ChooseTargetApp()
        {
            if (OpenWithPreselectedAppText.IsPresent())
            {
                UseSelectedApp();
            }
            else if (OpenWithAppChoiceText.IsPresent())
            {
                SelectAndUseApp();
            }
        }

        private void SelectAndUseApp()
        {
            TargetAppChoice.Click();
            JustOnceButton.Click();
        }

        private void UseSelectedApp() => JustOnceButton.Click();
    }
}