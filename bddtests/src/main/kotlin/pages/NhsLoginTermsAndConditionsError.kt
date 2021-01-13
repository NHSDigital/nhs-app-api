package pages

class NhsLoginTermsAndConditionsError: HybridPageObject() {

    private val desktopLocator = "//div[@id='termsAndConditionsError']"

    fun assertTitle(): NhsLoginTermsAndConditionsError{
        val title = getElement(
                "//h1[contains(text(),\"You need to accept NHS login terms of use to continue\")]")
        title.assertIsVisible()
        return this
    }

    fun assertParagraphText(paragraphText: String): NhsLoginTermsAndConditionsError {
        val message = getElement("$desktopLocator//p[normalize-space()=\"$paragraphText\"]")
        message.assertIsVisible()
        return this
    }
}
