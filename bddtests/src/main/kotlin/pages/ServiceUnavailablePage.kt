package pages

class ServiceUnavailablePage : HybridPageObject() {

    private val header = HybridPageElement(
            browserLocator = "//div/h2",
            androidLocator = null,
            page = this,
            helpfulName = "content"
    ).withText("You cannot currently use this service", false)

    private val content = HybridPageElement(
            browserLocator = "//div/p",
            androidLocator = null,
            page = this,
            helpfulName = "error message"
    )

    fun assertIsPresent(message: String) {
        header.assertSingleElementPresent()
        content.withText(message, false).assertSingleElementPresent()
    }
}