package pages

import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import worker.models.ErrorCodeParagraph

class ErrorDialogPage : HybridPageObject() {
    private val errorContainerLocator = "//div[@data-purpose='error']"
    private val errorTextFinderFormat = "$errorContainerLocator//p[@data-purpose='%s']"

    private val messageTextLocator = String.format(errorTextFinderFormat, "msg-text")

    fun assertPageTitle(title: String): ErrorDialogPage {
        assertEquals("$title - NHS App", this.title)
        return this
    }

    override fun assertPageHeader(headerText: String): ErrorDialogPage {
        super.assertPageHeader(headerText)
        return this
    }

    fun assertParagraphText(paragraphText: String): ErrorDialogPage {
        val message = getElement("$messageTextLocator[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }

    fun assertReferenceCode(referenceCode: String): ErrorDialogPage {
        val message = getElement("//div[contains(text(), 'Reference: $referenceCode')]")
        message.assertIsVisible()
        return this
    }

    fun assertSubHeader(paragraphText: String): ErrorDialogPage {
        val message = getElement("//h2[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }

    fun assertParagraphText(paragraph: ErrorCodeParagraph): ErrorDialogPage {
        val locator = "$messageTextLocator[starts-with(normalize-space(),'${paragraph.startText}')]"
        val message = getElement(locator)
        message.assertIsVisible()

        val regex = ("${Regex.escape(paragraph.startText)} " +
                "${paragraph.errorCodePrefix}\\w{4} " +
                "${Regex.escape(paragraph.endText)}")
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
}
