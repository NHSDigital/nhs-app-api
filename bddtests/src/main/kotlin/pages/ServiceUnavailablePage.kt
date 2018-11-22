package pages

class ServiceUnavailablePage : HybridPageObject() {

    private val header = HybridPageElement(
            browserLocator = "//div/h2",
            androidLocator = null,
            page = this,
            helpfulName = "content"
    )

    private val content = HybridPageElement(
            browserLocator = "//div/p",
            androidLocator = null,
            page = this,
            helpfulName = "error message"
    )

    fun assertIsPresent(titleText: String, message: String) {
        header.withText(titleText, true).assertSingleElementPresent()
        content.withText(message, false).assertSingleElementPresent()
    }
}