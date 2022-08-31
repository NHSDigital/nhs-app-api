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
            this,
            helpfulName = "header")
        header.waitForElement()
    }

    fun assertTitleDisplayed(id: String) {
        val path = "//p[@id='${id}']"
        val h5 = HybridPageElement(
                path,
                this,
                helpfulName = "unreadCountTitle")
        h5.waitForElement()
    }
}
