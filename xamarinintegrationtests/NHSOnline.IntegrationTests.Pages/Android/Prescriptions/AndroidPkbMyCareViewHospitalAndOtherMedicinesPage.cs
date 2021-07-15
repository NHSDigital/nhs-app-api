using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public class AndroidPkbMyCareViewHospitalAndOtherMedicinesPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFullNavigation Navigation { get; }

        private PkbMyCareViewHospitalAndOtherMedicinesPageContent PageContent { get; }

        private AndroidPkbMyCareViewHospitalAndOtherMedicinesPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PkbMyCareViewHospitalAndOtherMedicinesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidPkbMyCareViewHospitalAndOtherMedicinesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidPkbMyCareViewHospitalAndOtherMedicinesPage(driver);
            page.PageContent.AssertOnPage();
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