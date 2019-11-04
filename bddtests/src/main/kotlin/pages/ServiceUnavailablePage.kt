package pages

class ServiceUnavailablePage : HybridPageObject() {

    private val header = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'Login failed')]",
            androidLocator = null,
            page = this,
            helpfulName = "content"
    )

    private val content = HybridPageElement(
            webDesktopLocator = "//div/p",
            androidLocator = null,
            page = this,
            helpfulName = "error message"
    )

    fun assertIsPresent(titleText: String, message: String) {
        assert(titleText.equals(titleText))
        assert(header.isVisible)
        content.withText(message, false).assertSingleElementPresent()
    }

}
