package pages.messages

import constants.DateTimeFormats
import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import worker.models.messages.MessageFacade
import java.time.LocalDate
import java.time.format.DateTimeFormatter

class MessageBlockElements(private val page:HybridPageObject) {

    private fun messagesXpathAboveUnreadLine(aboveUnreadLine: Boolean): String {
        val followingOrPrecedingUnread = if (aboveUnreadLine) "following" else  "preceding"
        return "//ul[$followingOrPrecedingUnread-sibling::div/span[normalize-space(text())='Unread messages']]/li/div"
    }

    fun assertUnreadMessages(expectedMessages: ArrayList<MessageFacade>) {
        val actualUnreadMessages = getMessages(page, messagesXpathAboveUnreadLine(false))
        assertMessages(expectedMessages, actualUnreadMessages, "unread")
    }

    fun assertReadMessages(expectedMessages: ArrayList<MessageFacade>) {
        val actualReadMessages = getMessages(page, messagesXpathAboveUnreadLine(true))
        assertMessages(expectedMessages, actualReadMessages, "read")
    }

    private fun assertMessages(expectedMessages: ArrayList<MessageFacade>,
                               actualMessages: List<MessageBlockElement>,
                               messageType: String) {
        Assert.assertEquals("Expected $messageType messages", expectedMessages.count(), actualMessages.count())
        actualMessages.forEach { actualMessage -> assertDateFormat(actualMessage) }

        val actualMessagesToAssert = actualMessages.map { message -> message.toFacade() }
        expectedMessages.forEach { expectedMessage ->
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

        fun toFacade(): MessageFacade {
            return MessageFacade(messageBody, sender)
        }
    }
}