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

    fun assertCorrectErrorMessageShown(
            pageHeaderText: String? = null,
            headerText: String,
            subHeaderText: String,
            messageText: String? = null,
            retryButtonText: String? = null,
            errorDetailText: String? = null
    ) {
        assertEquals("Content header incorrect. ", headerText, heading.element.text)
        if (!subHeaderText.isNullOrEmpty()) assertEquals("Content sub-header incorrect. ", subHeaderText, subHeading.element.text)
        if (!messageText.isNullOrEmpty()) assertEquals("Content message incorrect. ", messageText, errorText1.element.text)
        if (!errorDetailText.isNullOrEmpty()) assertEquals("Content message incorrect. ", errorDetailText, errorText2.element.text)
        if (retryButtonText.isNullOrEmpty()) {
            button.assertElementNotPresent()
        } else {
            assertEquals("Retry button text incorrect. ", retryButtonText, button.element.text)
        }
        // Asserting page header last, as it's generic, whereas the content header and subheader are specific to errors
        if (!pageHeaderText.isNullOrEmpty()) waitForPageHeaderText(pageHeaderText!!)
    }
}