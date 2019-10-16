package pages

class ServiceUnavailablePage : HybridPageObject() {

    private val header = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'You are too young to use the NHS App')]",
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
