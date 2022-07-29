package pages.messages

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject

private const val SPAN_COUNT_WITH_UNREAD: Int = 2

class SenderBlockElements(private val page: HybridPageObject) {

    fun assertAll(expectedSenders: ArrayList<worker.models.messages.SenderFacade>) {
        val actualSenders = getAll().map { inboxMessage -> inboxMessage.toFacade() }
        Assert.assertEquals("Expected senders", expectedSenders.count(), actualSenders.count())

        val expectedSendersToAssert = expectedSenders.map { sender ->
            SenderFacade(sender.name, sender.unreadCount)
        }

        expectedSendersToAssert.forEachIndexed { index, expected ->
            expected.assertEquals(actualSenders.elementAt(index))
        }
    }

    fun assertUnread(senderName: String) {
        Assert.assertTrue("Expected sender to be unread", get(senderName).unreadCount > "0")
    }

    fun assertRead(senderName: String) {
        Assert.assertEquals("Expected sender to be read", get(senderName).unreadCount, "0")
    }

    private fun getAll(): List<SenderBlockElement> {
        val locator = "//ul[@id='inboxMessages']/li/a"
        val messagesLocator = HybridPageElement(
            locator,
            page = page,
            helpfulName = "Messages",
            timeToWaitForElement = 100
        )
        return messagesLocator.getElements { element -> SenderBlockElement(element) }
    }

    fun select(senderName: String) {
        get(senderName).click()
    }

    private fun get(senderName: String) : SenderBlockElement {
        return getAll().first { sender -> sender.name == senderName }
    }

    private class SenderBlockElement(private val element: WebElementFacade) {
        private val headerElement = element.findElement<WebElement>(By.xpath(".//h2"))
        private val spanElements = element.findElements<WebElement>(By.xpath(".//span"))

        private val hasUnread = spanElements.count() == SPAN_COUNT_WITH_UNREAD

        val name: String = headerElement.text
        val unreadCount: String = if (hasUnread) spanElements.last().text else "0"

        fun click() {
            element.click()
        }

        fun toFacade(): SenderFacade {
            return SenderFacade(name, unreadCount)
        }
    }

    private class SenderFacade(
        val sender: String,
        val unreadCount: String
    ) {

        override fun toString(): String {
            return "sender: $sender\n" +
                    "unreadCount: $unreadCount\n"
        }

        fun assertEquals(actual: SenderFacade) {
            Assert.assertEquals("sender", sender, actual.sender)
            Assert.assertEquals("unreadCount", unreadCount, actual.unreadCount)
        }
    }
}
