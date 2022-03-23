using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public class AndroidChooseRepeatPrescriptionPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFullNavigation Navigation { get; }

        public ChoosePrescriptionPageContent PageContent { get; }


        private AndroidChooseRepeatPrescriptionPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new ChoosePrescriptionPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidChooseRepeatPrescriptionPage AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidChooseRepeatPrescriptionPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidChooseRepeatPrescriptionPage));
            }

            return page;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardHomeNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardHomeNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public void KeyboardNavigateBack() =>
            PageContent.KeyboardNavigateBack(KeyboardPageContentNavigation);
    }
}