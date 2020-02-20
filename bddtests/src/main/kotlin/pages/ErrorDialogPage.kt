package pages

import org.junit.Assert
import pages.navigation.WebHeader
import worker.models.ErrorCodeParagraph

class ErrorDialogPage : HybridPageObject() {
    private val errorContainerLocator = "//div[@data-purpose='error']"
    private val errorTextFinderFormat = "$errorContainerLocator//p[@data-purpose='%s']"

    private val messageTextLocator = String.format(errorTextFinderFormat, "msg-text")
    private val pageHeader = findElementByLocator("//div[@id='content-header']//h1")

    lateinit var webHeader: WebHeader

    fun assertPageTitle(pageTitleText: String): ErrorDialogPage {
        webHeader.getPageTitle().withText(pageTitleText)
        return this
    }

    fun assertPageHeader(pageHeaderText: String): ErrorDialogPage {
        Assert.assertEquals("Page header incorrect. ", pageHeaderText, pageHeader.text)
        return this
    }

    fun assertParagraphText(paragraphText: String): ErrorDialogPage {
        val message = findElementByLocator("$messageTextLocator[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }

    fun assertParagraphText(paragraph: ErrorCodeParagraph): ErrorDialogPage {
        val locator = "$messageTextLocator[starts-with(normalize-space(),'${paragraph.startText}')]"
        val message = findElementByLocator(locator)
        message.assertIsVisible()

        val regex = ("${Regex.escape(paragraph.startText)} " +
                "${paragraph.errorCodePrefix}\\w{4} " +
                "${Regex.escape(paragraph.endText)}")
                .toRegex()

        Assert.assertTrue("Expected '${message.text}' text to match '${regex.toPattern()}' pattern",
                regex.matches(message.text))
        return this
    }

    fun assertLink(linkText: String, url: String? = null) : HybridPageElement {
        var locator = "$errorContainerLocator//a[contains(text(),'$linkText')]"
        var message: String? = null

        if (!url.isNullOrBlank()) {
            locator += "[starts-with(@href, '$url')]"
            message = "Expected the link called $linkText with target of $url to be available"
        }
        return findElementByLocator(locator, message).assertIsVisible()
    }

    private fun findElementByLocator(locator: String, iOSLocator: String? = null,
                                     androidLocator: String? = null): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = locator,
                iOSLocator = iOSLocator,
                androidLocator = androidLocator,
                page = this
        )
    }
}