package pages.patientPracticeMessaging

import org.junit.Assert.assertEquals
import models.ExpectedMessage
import org.junit.Assert.assertTrue
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed
import pages.text

class PatientPracticeMessagingPage: HybridPageObject() {

    private var baseMessagePath: String = ""

    fun assertDisplayed() {
        val path = "//h1[normalize-space(text())='Messages']"
        val header = HybridPageElement(
                path,
                path,
                null,
                null,
                this,
                helpfulName = "header")
        header.waitForElement()
    }

    fun assertCorrectMessagesDisplayed(expectedMessages: List<ExpectedMessage>){
        for (expectedMessage in expectedMessages) {
            baseMessagePath = "//a[@id='${expectedMessage.id}']"

            assertEquals(getMessageTitle().text, expectedMessage.recipient)
            assertEquals(getMessageDate().text, expectedMessage.lastMessageDateTime)
            assertEquals(getMessageSubject().text, expectedMessage.subject)
        }
    }

    fun clickFirstMessage() {
        HybridPageElement(
                webDesktopLocator = "//a[@id='1']",
                androidLocator = null,
                page= this
        ).click()
    }

    fun assertNoMessagesTextDisplayed() {
        assertTrue("Expected No patient practice messages to be displayed", noMessagesText().isDisplayed)
    }

    private fun noMessagesText(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//p[contains(text(),'You have no messages')]",
                androidLocator = null,
                page = this
        )
    }

    private fun getMessageDate(): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "$baseMessagePath//span",
                androidLocator = null,
                page = this
        )
    }

    private fun getMessageSubject(): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "$baseMessagePath//p",
                androidLocator = null,
                page = this)
    }

    private fun getUnreadCount(): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "$baseMessagePath//p",
                androidLocator = null,
                page = this)
    }

    private fun getMessageTitle(): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "$baseMessagePath//h2",
                androidLocator = null,
                page = this)
    }

}