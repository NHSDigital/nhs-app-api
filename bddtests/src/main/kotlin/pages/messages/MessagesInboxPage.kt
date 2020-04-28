package pages.messages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertSingleElementPresent

@DefaultUrl("http://web.local.bitraft.io:3000/messages/messaging")
class MessagesInboxPage : HybridPageObject() {
    val messages by lazy { InboxSummaryMessageBlockElements(this) }

    fun assertHeaderDisplayed() {
        val path = "//h1[normalize-space(text())='Health information and updates']"
        val header = HybridPageElement(
                path,
                path,
                null,
                null,
                this,
                helpfulName = "header")
        header.waitForElement()
    }

    fun assertSubHeaderDisplayed() {
        val path = "//h2[normalize-space(text())='Your messages']"
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
        noMessages.assertSingleElementPresent()
    }
}