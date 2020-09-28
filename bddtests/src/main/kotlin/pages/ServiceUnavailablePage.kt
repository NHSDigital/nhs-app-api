package pages

import org.junit.Assert

class ServiceUnavailablePage : HybridPageObject() {

    private val header = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'Login failed')]",
            androidLocator = null,
            page = this,
            helpfulName = "content"
    )

    private val content = HybridPageElement(
            webDesktopLocator = "//p[@data-purpose='msg-text']",
            androidLocator = null,
            page = this,
            helpfulName = "error message"
    )

    fun assertIsPresent(titleText: String, message: String) {
        assert(titleText.equals(header.textValue))
        header.assertIsVisible()
        Assert.assertEquals(message, content.text)
    }
}
