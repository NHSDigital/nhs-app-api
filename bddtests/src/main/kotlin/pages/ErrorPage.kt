package pages

import org.junit.Assert.assertEquals

class ErrorPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {
    private val errorTextFinderFormat = "//div[@data-purpose='error']/p[@data-purpose='%s']"

    private val headerLocator = String.format(errorTextFinderFormat, "msg-header")
    private val subHeaderLocator = String.format(errorTextFinderFormat, "msg-subheader")
    private val messageTextLocator = String.format(errorTextFinderFormat, "msg-text")
    private val extraMessageTextLocator = String.format(errorTextFinderFormat, "msg-extratext")
    private val backButtonLocator = "//button[@data-purpose='retry-or-back-button']"

    val heading = findElementByLocator(headerLocator)

    val subHeading = findElementByLocator(subHeaderLocator)

    val errorText1 = findElementByLocator(messageTextLocator)

    val errorText2 = findElementByLocator(extraMessageTextLocator)

    val button = findElementByLocator(backButtonLocator)

    private fun findElementByLocator(locator: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = locator,
                androidLocator = null,
                page = this
        )
    }

    @Suppress("TooGenericExceptionCaught")
    fun hasButton(text: String): Boolean {
        return try {
            button.element.text == text
        } catch (e: Exception) {
            false
        }
    }

    fun assertPageHeader(pageHeaderText: String): ErrorPage {
        waitForPageHeaderText(pageHeaderText)
        return this
    }

    fun assertHeaderText(headerText: String): ErrorPage {
        assertEquals("Content header incorrect. ", headerText, heading.element.text)
        return this
    }

    fun assertSubHeaderText(subHeaderText: String): ErrorPage {
        assertEquals("Content sub-header incorrect. ", subHeaderText, subHeading.element.text)
        return this
    }

    fun assertMessageText(messageText: String): ErrorPage {
        assertEquals("Content message incorrect. ", messageText, errorText1.element.text)
        return this
    }

    fun assertRetryButtonText(retryButtonText: String): ErrorPage {
        assertEquals("Retry button text incorrect. ", retryButtonText, button.element.text)
        return this
    }

    fun assertNoRetryButton(): ErrorPage{
        button.assertElementNotPresent()
        return this
    }

    fun assertErrorDetailText(errorDetailText: String): ErrorPage {
        assertEquals("Content message incorrect. ", errorDetailText, errorText2.element.text)
        return this
    }
}