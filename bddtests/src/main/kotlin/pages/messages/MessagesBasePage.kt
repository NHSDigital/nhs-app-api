package pages.messages

import net.thucydides.core.annotations.NotImplementedException
import pages.HybridPageElement
import pages.HybridPageObject

open class MessagesBasePage : HybridPageObject() {
    open val titleText: String
        get() {
            throw NotImplementedException("titleText is not implemented on Messages base page")
        }

    fun assertDisplayed(sender: String) {
        val path = "//h1[contains(normalize-space(), '$sender')]//span[contains(normalize-space(), '$titleText:')]"
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
