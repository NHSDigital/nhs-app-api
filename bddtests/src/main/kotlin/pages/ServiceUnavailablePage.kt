package pages

class ServiceUnavailablePage : HybridPageObject() {

    private val header = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'Login failed')]",
            androidLocator = null,
            page = this,
            helpfulName = "content"
    )

    fun assertTitle(titleText: String): ServiceUnavailablePage {
        assert(titleText.equals(header.textValue))
        header.assertIsVisible()
        return this
    }

    fun assertParagraphText(paragraphText: String): ServiceUnavailablePage {
        val message = getElement("//p[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }
}
