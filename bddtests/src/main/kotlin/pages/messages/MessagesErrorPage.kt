package pages.messages

import org.junit.Assert
import pages.ErrorPage
import pages.HybridPageElement

class MessagesErrorPage: ErrorPage() {

    private val pageTitle = "Messages error"
    private val errorHeader = "There is a problem getting your messages"
    private val tryAgain = "Try again now."
    private val retryButton = "Try again"

    private val errorText = HybridPageElement(
            webDesktopLocator = "//div[@data-purpose='error']//p",
            page = this
    )

    private val title by lazy {
        HybridPageElement(
                webDesktopLocator ="//h1[normalize-space(text())='$pageTitle']",
                page = this,
                helpfulName = "header")
    }

    fun assertInboxErrorPage() {
        title.waitForElement()
        assertHeaderText(errorHeader)
        assertMessageText(tryAgain)
        assertRetryButtonText(retryButton)
    }

    fun assertSenderErrorPage(sender: String) {
        title.waitForElement()
        assertHeaderText(errorHeader)
        assertMessageText(tryAgain)
        assertRetryButtonText(retryButton)
        val content = errorText.elements.map { element -> element.textValue.trim() }.toTypedArray()
        Assert.assertArrayEquals("Expected error content",
                arrayOf(
                        errorHeader,
                        tryAgain,
                        "If the problem continues and you need this information now, contact $sender directly."),
                content)
    }
}
