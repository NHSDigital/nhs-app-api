package pages.throttling

import pages.HybridPageObject
import pages.HybridPageElement

class SendingEmailResultsPage : HybridPageObject() {

    val waitingListResultsHeader = HybridPageElement(
            browserLocator = "//h1[contains(text(), 'Waiting list')]",
            androidLocator = null,
            page = this
    )

    val letYouKnowText =
            HybridPageElement(
                    browserLocator = "//h3[contains(text(), 'We will let you know')]",
                    androidLocator = null,
                    page = this
            )

    val gpSurgeryFeatureText =
            HybridPageElement(
                    browserLocator = "//p[contains(text(), 'When your GP surgery can use all features of the app, " +
                            "we’ll send you an email.')]",
                    androidLocator = null,
                    page = this
            )

    val homeButton = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Go to home screen')]",
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
