package pages.throttling

import pages.HybridPageObject
import pages.HybridPageElement

class WaitingListJoinedPage : HybridPageObject() {

    val waitingListResultsHeader = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'Next steps')]",
            webMobileLocator = "//h1[contains(text(), 'Next steps')]",
            androidLocator = null,
            page = this
    )

    val letYouKnowText =
            HybridPageElement(
                    webDesktopLocator = "//h3[contains(text(), 'We will let you know')]",
                    webMobileLocator = "//h3[contains(text(), 'We will let you know')]",
                    androidLocator = null,
                    page = this
            )

    val gpSurgeryFeatureText =
            HybridPageElement(
                    webDesktopLocator = "//p[contains(text(), 'When your GP surgery can use all features of the app, " +
                            "we’ll send you an email.')]",
                    webMobileLocator = "//p[contains(text(), 'When your GP surgery can use all features of the app, " +
                            "we’ll send you an email.')]",
                    androidLocator = null,
                    page = this
            )

    val homeButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Go to home screen')]",
            webMobileLocator = "//button[contains(text(), 'Go to home screen')]",
            androidLocator = null,
            page = this
    )

    val whatNextTitle = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'What happens next')]",
            webMobileLocator = "//h2[contains(text(), 'What happens next')]",
            androidLocator = null,
            page = this
    )

    val whatNextJoinedParagraph = HybridPageElement(
            webDesktopLocator = "//p[contains(text(), \"We've just sent you an email. " +
                    "You need to confirm you want to " +
                    "be updated by us about your GP surgery. You do this by following a " +
                    "link in that email. It may be  in your junk folder.\")]",
            webMobileLocator = "//p[contains(text(), \"We've just sent you an email. " +
                    "You need to confirm you want to " +
                    "be updated by us about your GP surgery. You do this by following a " +
                    "link in that email. It may be  in your junk folder.\")]",
            androidLocator = null,
            page = this
    )

    val whatNextNotJoinedParagraph = HybridPageElement(
            webDesktopLocator = "//p[contains(text(), \"Check in with your GP surgery to find out when they'll be " +
                    "using all the features of the app. When they are, they will help you set up an NHS login.\")]",
            webMobileLocator = "//p[contains(text(), \"Check in with your GP surgery to find out when they'll be " +
                    "using all the features of the app. When they are, they will help you set up an NHS login.\")]",
            androidLocator = null,
            page = this
    )

    val whatToDoTitle = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'What you can do until then')]",
            webMobileLocator = "//h2[contains(text(), 'What you can do until then')]",
            androidLocator = null,
            page = this
    )


    fun isWaitingListResultsHeaderVisible(): Boolean {
        return waitingListResultsHeader.element.isDisplayed
    }

    fun isLetYouKnowTextVisible(): Boolean {
        return letYouKnowText.element.isDisplayed
    }

    fun gpSurgeryFeatureText(): Boolean {
        return gpSurgeryFeatureText.element.isDisplayed
    }

    fun clickHomeButtonButton() {
        homeButton.element.click()
    }
}
