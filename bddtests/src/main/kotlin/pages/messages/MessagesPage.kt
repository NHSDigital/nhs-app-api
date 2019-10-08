package pages.messages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/more/messaging/messages")
class MessagesPage : HybridPageObject() {

    val messages = MessageBlockElements(this)

    fun assertDisplayed(sender: String) {
        assertTitle(sender)
    }

    private fun assertTitle(sender: String) {
        val header = HybridPageElement(
                "//h1",
                "//h1",
                null,
                null,
                this,
                helpfulName = "header")
        Assert.assertEquals("Header text", "Messages from:\n$sender", header.textValue)
    }
}