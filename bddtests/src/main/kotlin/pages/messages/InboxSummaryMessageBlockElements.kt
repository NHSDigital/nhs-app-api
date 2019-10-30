package pages.messages

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import worker.models.messages.MessagesSummaryFacade


private const val HEADER_SENDER_INDEX = 0
private const val HEADER_TIME_INDEX = 1

private const val BODY_UNREAD_INDEX = 0
private const val BODY_MESSAGE_INDEX = 1

class InboxSummaryMessageBlockElements(private val page: HybridPageObject) {

    fun assertMessages(expectedMessages: ArrayList<MessagesSummaryFacade>) {
        val actualMessages = getMessages()
        Assert.assertEquals("Expected messages", expectedMessages.count(), actualMessages.count())

        val expectedInboxMessages = expectedMessages.flatMap { inboxMessage ->
            inboxMessage.messages.map { m ->
                InboxMessageFacade(m.sender, m.body, inboxMessage.lastMessageTime!!, inboxMessage.unreadCount)
            }
        }

        val actualMessagesToAssert = actualMessages.map { inboxMessage -> inboxMessage.toFacade() }

        expectedInboxMessages.forEach { expected ->
            expected.assertEquals(
                    actualMessagesToAssert.single { inboxMessage -> inboxMessage.sender == expected.sender })
        }
    }

    private fun getMessages(): List<InboxMessageBlockElement> {
        val locator = "//ul/li/a"
        val messagesLocator = HybridPageElement(
                locator,
                locator,
                page = page,
                helpfulName = "Messages",
                timeToWaitForElement = 100)
        return messagesLocator.getElements { element -> InboxMessageBlockElement(element) }
    }

    fun selectMessageSummary(targetSender: String) {
        getMessages().first { inboxMessage -> inboxMessage.sender == targetSender }.click()
    }

    private class InboxMessageBlockElement(private val element: WebElementFacade) {

        private val headerElements = element.findElement<WebElement>(By.xpath("./h2")).text.split("\n")
        private val bodyElements = element.findElement<WebElement>(By.xpath("./p")).text.split("\n")

        val sender: String = headerElements[HEADER_SENDER_INDEX]
        val sentTime: String = headerElements[HEADER_TIME_INDEX]
        val messageBody: String = bodyElements[BODY_MESSAGE_INDEX]
        val unreadCount: Int? = bodyElements[BODY_UNREAD_INDEX].toInt()

        fun click() {
            element.click()
        }

        fun toFacade(): InboxMessageFacade {
            return InboxMessageFacade(sender, messageBody, sentTime, unreadCount)
        }
    }

    private class InboxMessageFacade(
            val sender: String,
            val messageBody: String,
            val sentTime: String,
            val unreadCount: Int?) {

        override fun toString(): String {
            return "sender: $sender\n" +
                    "messageBody: $messageBody\n" +
                    "sentTime: $sentTime\n" +
                    "unreadCount: $unreadCount\n"
        }

        fun assertEquals(actual: InboxMessageFacade) {
            Assert.assertEquals("sender", sender, actual.sender)
            Assert.assertEquals("messageBody", messageBody, actual.messageBody)
            Assert.assertEquals("sentTime", sentTime, actual.sentTime)
            Assert.assertEquals("unreadCount", unreadCount, actual.unreadCount)
        }
    }
}