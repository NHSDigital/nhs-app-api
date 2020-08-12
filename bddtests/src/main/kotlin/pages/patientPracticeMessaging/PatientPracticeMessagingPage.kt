package pages.patientPracticeMessaging

import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import models.ExpectedMessage
import org.junit.Assert.assertEquals
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsDisplayed
import pages.text
import pages.assertIsVisible
import utils.getOrNull

class PatientPracticeMessagingPage: HybridPageObject() {

    private var baseMessagePath: String = ""

    fun clickSendAMessageButton() {
         HybridPageElement(
                 "//a[@id='sendMessageButton']",
                 page = this,
                 helpfulName = "Send a message button")
             .waitForElement()
             .click()
    }

    fun assertDisplayed() {
        val path = "//h1[normalize-space(text())='GP surgery messages']"
        val header = HybridPageElement(
                path,
                path,
                null,
                null,
                this,
                helpfulName = "header")
        header.waitForElement()
    }

    fun assertCorrectMessagesDisplayed(expectedMessages: List<ExpectedMessage>,
                                       hasSubject: Boolean = true,
                                       hasUnreadCount: Boolean = false,
                                       fromGP: Boolean = false) {
        expectedMessages.filter { m -> m.conversationId === m.id }
            .forEach { message ->
                baseMessagePath = "//a[@id='${message.id}']"

                if (!fromGP) {
                    assertEquals(getMessageTitle().text, message.recipient)
                } else {
                    assertEquals(getMessageTitle().text, message.sender)
                }

                assertEquals(getMessageDate().text, message.lastMessageDateTime)

                if (hasSubject) {
                    assertEquals(getMessageSubject().text, message.subject)
                }

                if (hasUnreadCount) {
                    assertEquals(
                        getUnreadIndicator().textValue,
                        message.unreadCount.toString())
                }

                if (message.hasUnreadReplies!!) {
                    getUnreadIndicator().assertIsVisible()
                }
            }
    }

    fun clickFirstMessage() {
        HybridPageElement(
                webDesktopLocator = "//a[@id='${PatientPracticeMessagingSerenityHelpers.INITIAL_MESSAGE_ID
                        .getOrNull<String>()}']",
                androidLocator = null,
                page= this
        ).click()
    }

    fun assertNoMessagesTextDisplayed() {
        noMessagesText().assertIsDisplayed("Expected No patient practice messages to be displayed")
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

    private fun getUnreadIndicator(): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "//*[@id='unreadIndicator0']",
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
