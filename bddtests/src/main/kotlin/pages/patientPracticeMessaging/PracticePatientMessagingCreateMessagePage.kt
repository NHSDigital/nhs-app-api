package pages.patientPracticeMessaging

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed
import org.junit.Assert.assertEquals
import pages.text

class PracticePatientMessagingCreateMessagePage: HybridPageObject() {

    fun assertDisplayed(assertSubject: Boolean) {
        messageTextInput().isDisplayed
        if (assertSubject) {
            subjectTextInput().isDisplayed
        }
        messageSubHeader().isDisplayed
        sendMessageButton().isDisplayed

        assertEquals("For advice now, contact your GP surgery directly, go to 111.nhs.uk or call 111.",
                messageSubHeader().text)
    }

    fun assertHeaderContainsRecipient(name: String){
        header().text.contains(name)
    }

    fun assertValidationErrorsDisplayed() {
        validationBanner.assertMessageItem("Enter a subject").assertMessageItem("Enter a message")
    }

    fun insertMessageText(message: String) {
        messageTextInput().actOnTheElement { it.type<WebElementFacade>(message) }
    }

    fun insertSubjectText(subject: String) {
        subjectTextInput().actOnTheElement { it.type<WebElementFacade>(subject) }
    }

    fun sendMessage() {
        sendMessageButton().click()
    }

    private fun header(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//h1",
                androidLocator = null,
                page = this,
                helpfulName = "Header")
    }

    private fun pageElement(xPath: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = xPath,
                androidLocator = null,
                page = this
        )
    }

    private fun messageSubHeader(): HybridPageElement {
        return pageElement("//p[@id='subHeader']")
    }

    private fun subjectTextInput(): HybridPageElement {
        return pageElement("//input[@id='subjectText']")
    }

    private fun messageTextInput() : HybridPageElement {
        return pageElement("//textarea[@id='messageText']")
    }

    private fun sendMessageButton(): HybridPageElement {
        return pageElement("//button[@id='send_message_btn']")
    }
}
