package pages

class ServiceUnavailablePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val header = HybridPageElement(
            browserLocator = "//div/h2",
            androidLocator = null,
            page = this,
            helpfulName = "content"
    ).containingText("You cannot currently use this service")

    private val content = HybridPageElement(
            browserLocator = "//div/p",
            androidLocator = null,
            page = this,
            helpfulName = "error message"
    )

    fun assertIsPresent(message: String) {
        header.assertSingleElementPresent()
        content.containingText(message).assertSingleElementPresent()
    }
}