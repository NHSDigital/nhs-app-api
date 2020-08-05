package pages

open class GpSessionError: HybridPageObject() {

    override fun assertPageHeader(headerText: String): GpSessionError {
        super.assertPageHeader(headerText)
        return this
    }

    fun assertParagraphText(paragraphText: String): GpSessionError {
        val message = getElement("//p[contains(text(), '$paragraphText')]")
        message.assertIsVisible()
        return this
    }

    fun assertReferenceCode(referenceCode: String): GpSessionError {
        val message = getElement("//div[contains(text(), 'Reference: $referenceCode')]")
        message.assertIsVisible()
        return this
    }

    fun assertReportAProblemLink(): GpSessionError {
        val message = getElement("//a[contains(text(),'Report a problem')]" +
                "[starts-with(@href, 'https://www.nhs.uk/contact-us/nhs-app-contact-us')]")
        message.assertIsVisible()
        return this
    }

    fun clickReportAProblemLink() {
        getElement("//a[contains(text(),'Report a problem')]" +
                "[starts-with(@href, 'https://www.nhs.uk/contact-us/nhs-app-contact-us')]")
                .click()
    }

    fun clickBackLink() {
        HybridPageElement(
                webDesktopLocator = "//a[@data-purpose='error']",
                page = this
        ).click()
    }

   fun setupElement(headerText: String): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "//h2[contains(text(),'$headerText')]",
                androidLocator = null,
                page = this
        )
    }
}
