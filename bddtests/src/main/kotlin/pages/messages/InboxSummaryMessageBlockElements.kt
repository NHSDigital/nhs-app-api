package pages.messages

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import worker.models.messages.MessagesSummaryFacade

private const val SPAN_COUNT_WITH_UNREAD: Int = 2

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
        val locator = "//ul[@id='inboxMessages']/li/a"
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
        private val headerElement = element.findElement<WebElement>(By.xpath(".//h2"))
        private val paragraphElement = element.findElement<WebElement>(By.xpath(".//p"))
        private val timeElement = element.findElement<WebElement>(By.xpath(".//time"))
        private val spanElements = element.findElements<WebElement>(By.xpath(".//span"))

        private val hasUnread = spanElements.count() == SPAN_COUNT_WITH_UNREAD

        val sender: String = headerElement.text
        val sentTime: String = timeElement.text
        val messageBody: String = paragraphElement.text
        val unreadCount: Int? = if (hasUnread) spanElements.last().text.toInt() else 0

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
