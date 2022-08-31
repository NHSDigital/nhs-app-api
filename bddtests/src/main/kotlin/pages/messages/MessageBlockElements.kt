package pages.messages

import constants.DateTimeFormats
import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import worker.models.messages.SingleMessageFacade
import java.time.LocalDate
import java.time.format.DateTimeFormatter

class MessageBlockElements(private val page:HybridPageObject) {

    fun assertUnread(expectedMessages: ArrayList<SingleMessageFacade>) {
        val actualMessages = getAll()
        assert(expectedMessages, actualMessages.filter { message -> message.isUnread }, "unread")
    }

    fun assertRead(expectedMessages: ArrayList<SingleMessageFacade>) {
        val actualMessages = getAll()
        assert(expectedMessages, actualMessages.filter { message -> !message.isUnread }, "read")
    }

    fun assertAllRead(expectedMessages: ArrayList<SingleMessageFacade>) {
        val actualMessages = getAll()
        Assert.assertFalse(actualMessages.any { message -> message.isUnread })
        assert(expectedMessages, actualMessages, "read")
    }

    private fun assert(expectedMessages: ArrayList<SingleMessageFacade>,
                       actualMessages: List<MessageBlockElement>,
                       messageType: String) {
        Assert.assertEquals("Expected $messageType messages", expectedMessages.count(), actualMessages.count())
        actualMessages.forEach { actualMessage ->
            assertDateFormat(actualMessage)
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

    private fun getAll(): List<MessageBlockElement> {
        val messageElements = HybridPageElement(
            "//ul/li/a/div",
            page = page,
            helpfulName = "Messages",
            timeToWaitForElement = 1).elements
        return messageElements.mapIndexed { index, element -> MessageBlockElement(element, index) }
    }

    fun select(body: String) {
        getAll().first { message -> message.messageBody.contains(body) }.click()
    }

    private class MessageBlockElement(private val element: WebElementFacade, index: Int) {
        val messageBody: String = element.findElement<WebElement>(By.xpath("./div/p")).text
        val sentTime: String = element.findElement<WebElement>(By.xpath("./div/time")).text
        val isUnread: Boolean = element.containsElements(By.xpath("//*[@id='unreadIndicator${index}']"))

        fun click() {
            element.click()
        }
    }
}
