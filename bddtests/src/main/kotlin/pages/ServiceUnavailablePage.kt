package pages
import org.junit.Assert

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

    fun assertErrorCodeIsPresent( prefix: String) {
        val errorCode = HybridPageElement(
                webDesktopLocator = "//span[@id=\"errorCode\"]",
                androidLocator = null,
                page = this,
                helpfulName = "error message"
        )
        Assert.assertEquals("the error code prefix does not match the expected prefix",
                prefix.toLowerCase(), errorCode.textValue.substring(0,2).toLowerCase())
    }
}
