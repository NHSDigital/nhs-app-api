package pages.messages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertSingleElementPresent

@DefaultUrl("http://web.local.bitraft.io:3000/messages/app-messaging")
class MessageSendersPage : HybridPageObject() {
    val senders by lazy { SenderBlockElements(this) }

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

    fun assertNoSenders() {
        val noMessages = HybridPageElement(
                "//span[@id='noMessages']",
                page = this,
                helpfulName = "no messages tag"
        )
        noMessages.assertSingleElementPresent()
    }
}
