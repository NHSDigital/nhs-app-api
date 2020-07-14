package pages.patientPracticeMessaging

import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed

class PatientPracticeMessagingDeleteSuccessPage: HybridPageObject() {

    fun assertDisplayed() {
        goToMessagesLink().isDisplayed
    }

    private fun pageElement(xPath: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = xPath,
                androidLocator = null,
                page = this
        )
    }

    private fun goToMessagesLink() : HybridPageElement {
        return pageElement("//a[@data-purpose='main-back-button']")
    }

    fun clickBackToMessages() {
        goToMessagesLink().click()
    }
}
