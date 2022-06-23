package pages

class ServiceUnavailablePage : HybridPageObject() {

    private val ageUnder13Header = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'Cannot log in due to age restriction')]",
            page = this,
            helpfulName = "content"
    )

    fun assertAgeUnder13Title(titleText: String): ServiceUnavailablePage {
        assert(titleText.equals(ageUnder13Header.textValue))
        ageUnder13Header.assertIsVisible()
        return this
    }

    fun assertParagraphText(paragraphText: String): ServiceUnavailablePage {
        val message = getElement("//p[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }
}
