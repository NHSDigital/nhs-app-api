using FluentAssertions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidBrowserChoice
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _targetApp;
        private readonly string? _link;

        private string OptionalLinkText => _link != null ? $" {_link} links" : " links";

        private AndroidSystemLabel OpenWithRequiresAppChoiceText => AndroidSystemLabel.WhichMatches(_driver,
            $"Open({OptionalLinkText})? with(?! {_targetApp}).*");

        private AndroidSystemLabel OpenWithPreselectedAppText =>
            AndroidSystemLabel.WhichMatches(_driver, $"Open({OptionalLinkText})? with {_targetApp}");

        private AndroidAppChoiceOption TargetAppChoice => AndroidAppChoiceOption.WithText(_driver, _targetApp);

        private AndroidSystemButton JustOnceButton =>
            AndroidSystemButton.WhichMatches(_driver, "(JUST ONCE|Just once)");

        public AndroidBrowserChoice(IAndroidDriverWrapper driver, string targetApp, string? link = null)
        {
            _driver = driver;
            _targetApp = targetApp;
            _link = link;
        }

        public void AssertDisplayed()
        {
            IsDisplayed().Should().BeTrue("expected android app choice to be displayed");
        }

        public bool IsDisplayed() => OpenWithRequiresAppChoiceText.IsPresent() || OpenWithPreselectedAppText.IsPresent();

        public void ChooseTargetApp()
        {
            if (OpenWithPreselectedAppText.IsPresent())
            {
                UseSelectedApp();
            }
            else if (OpenWithRequiresAppChoiceText.IsPresent())
            {
                SelectAndUseApp();
            }
        }

        private void SelectAndUseApp()
        {
            TargetAppChoice.Click();

            TransitoryErrorHandler.HandleSpecificFailure()
                .Alternate(() =>
                    {
                        JustOnceButton.Click();
                    },
                    "No AndroidElement found matching ByAndroidUIAutomator(new UiSelector().className(\"android.widget.Button\").textMatches(\"(JUST ONCE|Just once)\"))",
                    () =>
                    {
                        IsDisplayed().Should().BeFalse("Expected if 'Just Once' can't be found that it has already closed the dialog");
                    });
        }

        private void UseSelectedApp() => JustOnceButton.Click();
    }
}