package pages.patientPracticeMessaging

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsDisplayed

class PatientPracticeMessagingDeleteSuccessPage: HybridPageObject() {

    fun assertDisplayed() {
        goToMessagesLink().assertIsDisplayed("Expected Go To Messages Link")
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
