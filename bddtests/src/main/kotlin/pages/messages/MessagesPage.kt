package pages.messages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertSingleElementPresent

@DefaultUrl("http://web.local.bitraft.io:3000/more/messaging/messages")
class MessagesPage : HybridPageObject() {

    val messages = MessageBlockElements(this)

    val backLink = HybridPageElement(
            "//a[@data-purpose='main-back-button']",
            page = this
    )

    fun assertDisplayed(sender: String) {
        val path = "//h1[contains(normalize-space(), '$sender')]//span[contains(normalize-space(), 'Messages from:')]"
        val header = HybridPageElement(
                path,
                path,
                null,
                null,
                this,
                helpfulName = "header")
        header.waitForElement()
    }

    fun assertBackLinkDisplayed() {
        backLink.assertSingleElementPresent()
    }
}
