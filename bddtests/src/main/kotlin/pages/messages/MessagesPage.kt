package pages.messages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/more/messaging/messages")
class MessagesPage : HybridPageObject() {

    val messages = MessageBlockElements(this)

    fun assertDisplayed(sender: String) {
        val path = "//h1[normalize-space(text())='$sender']" +
                "[span[normalize-space(text())='Messages from:']]"
        val header = HybridPageElement(
                path,
                path,
                null,
                null,
                this,
                helpfulName = "header")
        header.waitForElement()
    }
}