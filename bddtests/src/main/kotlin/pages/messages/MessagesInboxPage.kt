package pages.messages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertSingleElementPresent

@DefaultUrl("http://web.local.bitraft.io:3000/more/messaging/inbox")
class MessagesInboxPage : HybridPageObject() {

    private val pageTitle = "Messages"

    val messages by lazy { InboxSummaryMessageBlockElements(this) }

    fun assertDisplayed() {
        val path = "//h1[normalize-space(text())='$pageTitle']"
        val header = HybridPageElement(
                path,
                path,
                null,
                null,
                this,
                helpfulName = "header")
        header.waitForElement()
    }

    fun assertNoMessages() {
        val noMessages = HybridPageElement(
                "//span[@id='noMessages']",
                page = this,
                helpfulName = "no messages tag"
        )
        val allTextInMainPage = HybridPageElement(
                "//div[@id='mainDiv']",
                page = this,
                helpfulName = "container for entire page")
        noMessages.assertSingleElementPresent()
        Assert.assertEquals("All text in main page","You have no messages", allTextInMainPage.textValue)
    }
}