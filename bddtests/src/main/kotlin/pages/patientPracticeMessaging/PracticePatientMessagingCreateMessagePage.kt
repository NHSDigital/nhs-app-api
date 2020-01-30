package pages.patientPracticeMessaging

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed
import org.junit.Assert.assertEquals
import pages.text

class PracticePatientMessagingCreateMessagePage: HybridPageObject() {

    fun assertDisplayed() {
        messageTextInput().isDisplayed
        subjectTextInput().isDisplayed
        messageSubHeader().isDisplayed
        sendMessageButton().isDisplayed

        assertEquals("For advice now, phone or visit your GP surgery, visit 111.nhs.uk or call 111.",
                messageSubHeader().text)
    }

    fun assertHeaderContainsRecipient(name: String){
        header().text.contains(name)
    }

    fun assertValidationErrorsDisplayed() {
        val message = errorMessage("message")
        val subject = errorMessage("subject")

        message.isDisplayed
        subject.isDisplayed

        assertEquals("Enter a subject", subject.text)
        assertEquals("Enter a message", message.text)
    }

    fun insertSubjectAndMessageText(subject: String, message: String) {
        subjectTextInput().actOnTheElement { it.type<WebElementFacade>(subject) }
        messageTextInput().actOnTheElement { it.type<WebElementFacade>(message) }

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

    private fun errorMessage(input: String) : HybridPageElement {
        return pageElement("//li[@data-purpose='$input-error']//p")
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