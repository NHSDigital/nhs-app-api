package pages

import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.navigation.HeaderNative

open class ErrorPage : HybridPageObject() {
    private val errorTextFinderFormat = "//div[@data-purpose='error']/p[@data-purpose='%s']"

    private val headerLocator = String.format(errorTextFinderFormat, "msg-header")
    private val subHeaderLocator = String.format(errorTextFinderFormat, "msg-subheader")
    private val messageTextLocator = String.format(errorTextFinderFormat, "msg-text")
    private val extraMessageTextLocator = String.format(errorTextFinderFormat, "msg-extratext")
    private val backButtonLocator = "//button[@data-purpose='retry-or-back-button']"
    lateinit var headerNative: HeaderNative

    val heading = findElementByLocator(headerLocator)

    val subHeading = findElementByLocator(subHeaderLocator)

    val errorText1 = findElementByLocator(messageTextLocator)

    private val errorText2 = findElementByLocator(extraMessageTextLocator)

    val button = findElementByLocator(backButtonLocator)

    protected fun findElementByLocator(locator: String, androidLocator: String? = null): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = locator,
                androidLocator = androidLocator,
                page = this,
                timeToWaitForElement = 45
        )
    }

    fun assertNoButton(text: String) {
        button.withText(text).assertElementNotPresent()
    }

    fun assertHasButton(expectedText: String) {
        button.assertSingleElementPresent()
        Assert.assertEquals("Expected button text", expectedText, button.element.text)
    }

    fun assertPageHeader(pageHeaderText: String): ErrorPage {
        headerNative.waitForPageHeaderText(pageHeaderText)
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

    fun assertNoRetryButton(): ErrorPage {
        button.assertElementNotPresent()
        return this
    }

    fun assertErrorDetailText(errorDetailText: String): ErrorPage {
        assertEquals("Content message incorrect. ", errorDetailText, errorText2.element.text)
        return this
    }
}
