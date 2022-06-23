package pages

import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import worker.models.ErrorCodeParagraph

class ErrorDialogPage : HybridPageObject() {
    private val errorContainerLocator = "//div[@data-purpose='error']"
    private val errorTextFinderFormat = "$errorContainerLocator//p[@data-purpose='%s']"
    private val errorMessageTextLocator = String.format(errorTextFinderFormat, "msg-text")

    private val warningContainerLocator = "//div[@data-purpose='warning']"
    private val warningTextFinderFormat = "$warningContainerLocator//p[@data-purpose='%s']"
    private val warningMessageTextLocator = String.format(warningTextFinderFormat, "msg-text")

    private val shutterContainerLocator = "//div[@data-purpose='shutter']"
    private val shutterTextFinderFormat = "$shutterContainerLocator//p[@data-purpose='%s']"
    private val shutterTextLocator = String.format(shutterTextFinderFormat, "msg-text")

    private val goBackAndTryAgainText = "Go back and try again."
    private val goBackAndTryAgainUrl = "appointments/gp-appointments"

    fun assertPageTitle(title: String): ErrorDialogPage {
        var retryCount = (TIME_TO_WAIT_FOR_ELEMENT / ELEMENT_RETRY_TIME).toInt()
        while (retryCount > 0) {
            if ("$title - NHS App" != this.title) {
                retryCount--
                Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
            }
            else {
                break
            }
        }
        assertEquals("$title - NHS App", this.title)
        return this
    }

    override fun assertPageHeader(headerText: String): ErrorDialogPage {
        super.assertPageHeader(headerText)
        return this
    }

    fun assertParagraphText(paragraphText: String): ErrorDialogPage {
        val message = getElement("$errorMessageTextLocator[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }

    fun assertWarningParagraphText(paragraphText: String): ErrorDialogPage {
        val message = getElement("$warningMessageTextLocator[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }

    fun assertShutterParagraphText(paragraphText: String): ErrorDialogPage {
        val message = getElement("$shutterTextLocator[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }

    fun assertParagraphText(paragraph: ErrorCodeParagraph): ErrorDialogPage {
        val locator = "$errorMessageTextLocator[starts-with(normalize-space(),'${paragraph.startText}')]"
        return assertTextMatches(locator, paragraph);
    }

    private fun assertTextMatches(locator: String, paragraph: ErrorCodeParagraph): ErrorDialogPage {
        val message = getElement(locator)
        message.assertIsVisible()

        val regex = ("${Regex.escape(paragraph.startText)} "
                + "${paragraph.errorCodePrefix}\\w{4} "
                + Regex.escape(paragraph.endText))
            .toRegex()

        assertTrue("Expected '${message.text}' text to match '${regex.toPattern()}' pattern",
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
        return getElement(locator).assertIsVisible(message)
    }

    fun assertGoBackAndTryAgainLink() : HybridPageElement{
        return assertWarningLink(goBackAndTryAgainText, goBackAndTryAgainUrl);
    }

    fun assertWarningLink(linkText: String, url: String? = null) : HybridPageElement {
        var locator = "${warningContainerLocator}//a[contains(text(),'$linkText')]"
        var message: String? = null

        if (!url.isNullOrBlank()) {
            locator += "[starts-with(@href, '$url')]"
            message = "Expected the link called $linkText with target of $url to be available"
        }
        return getElement(locator).assertIsVisible(message)
    }

    fun assertLinkOnShutter(linkText: String, url: String? = null) : HybridPageElement {
        var locator = "${shutterContainerLocator}//a[contains(text(),'$linkText')]"
        var message: String? = null

        if (!url.isNullOrBlank()) {
            locator += "[starts-with(@href, '$url')]"
            message = "Expected the link called $linkText with target of $url to be available"
        }
        return getElement(locator).assertIsVisible(message)
    }

    fun assertLinkWithPrefixOnShutter(linkText: String, url: String? = null, prefix: String) : HybridPageElement {
        var locator = "$shutterContainerLocator//a[contains(text(),'$linkText')]"
        var message: String? = null

        if (!url.isNullOrBlank()) {
            locator += "[starts-with(@href, '$url')]"
            message = "Expected the link called $linkText with target of $url to be available"
        }

        // Assert that the prefix is contained in the link text
        val element = getElement(locator)
        assertTrue(getElement(locator).textValue.contains("$linkText $prefix"))

        return element.assertIsVisible(message)
    }

    fun assertHasButton(expectedText: String) : ErrorDialogPage {
        val element = getElement("//button[normalize-space(text())='$expectedText']")
        element.assertSingleElementPresent()
        return this
    }
}
