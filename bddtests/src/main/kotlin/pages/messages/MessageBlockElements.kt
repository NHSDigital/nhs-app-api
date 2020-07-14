package pages.messages

import constants.DateTimeFormats
import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import worker.models.messages.SingleMessageFacade
import java.time.LocalDate
import java.time.format.DateTimeFormatter

class MessageBlockElements(private val page:HybridPageObject) {

    private val unreadMessagesBar = "div/div/span[normalize-space(text())='Unread messages']"

    private fun messagesXpathAboveUnreadLine(aboveUnreadLine: Boolean): String {
        val followingOrPrecedingUnread = if (aboveUnreadLine) "following" else  "preceding"
        return "//ul[$followingOrPrecedingUnread-sibling::$unreadMessagesBar]/li/div"
    }

    fun assertUnreadMessages(expectedMessages: ArrayList<SingleMessageFacade>, expectedSender: String) {
        val actualUnreadMessages = getMessages(page, messagesXpathAboveUnreadLine(false))
        assertMessages(expectedMessages, actualUnreadMessages, "unread", expectedSender)
    }

    fun assertReadMessages(expectedMessages: ArrayList<SingleMessageFacade>, expectedSender: String) {
        val actualReadMessages = getMessages(page, messagesXpathAboveUnreadLine(true))
        assertMessages(expectedMessages, actualReadMessages, "read", expectedSender)
    }

    fun assertAllReadMessages(expectedMessages: ArrayList<SingleMessageFacade>, expectedSender: String) {
        HybridPageElement(
                "//$unreadMessagesBar",
                "//$unreadMessagesBar",
                page = page,
                helpfulName = "Unread Messages Bar").assertElementNotPresent()
        val actualReadMessages = getMessages(page,"//ul/li/div")
        assertMessages(expectedMessages, actualReadMessages, "read", expectedSender)
    }

    private fun assertMessages(expectedMessages: ArrayList<SingleMessageFacade>,
                               actualMessages: List<MessageBlockElement>,
                               messageType: String,
                               expectedSender: String) {
        Assert.assertEquals("Expected $messageType messages", expectedMessages.count(), actualMessages.count())
        actualMessages.forEach { actualMessage ->
            assertDateFormat(actualMessage)
            Assert.assertEquals("Sender", expectedSender, actualMessage.sender)
        }

        val actualMessagesToAssert = actualMessages.map { message -> message.messageBody }
        expectedMessages.map { message -> message.body }.forEach { expectedMessage ->
            Assert.assertTrue(
                    "Expected $messageType message missing: $expectedMessage. " +
                            "Actual $messageType messages: $actualMessages",
                    actualMessagesToAssert.contains(expectedMessage))
        }
    }

    private fun assertDateFormat(actualMessage: MessageBlockElement) {
        val actualDate = actualMessage.sentTime
        val time = "([1-9]|(1[0-2])):([0-5][0-9])(am|pm)"
        val date = LocalDate.now().format(DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat))
        Regex("Sent $time $date").matches(actualDate)
    }

    private fun getMessages(page: HybridPageObject, locator: String): List<MessageBlockElement> {
        val messageElements = HybridPageElement(
                locator,
                locator,
                page = page,
                helpfulName = "Messages",
                timeToWaitForElement = 1).elements
        return messageElements.map { element -> MessageBlockElement(element) }
    }

    private class MessageBlockElement(element: WebElementFacade) {
        val sender: String = element.findElement<WebElement>(By.xpath("./h3")).text
        val messageBody: String = element.findElement<WebElement>(By.xpath("./div/p")).text
        val sentTime: String = element.findElement<WebElement>(By.xpath("./time")).text
    }
}
