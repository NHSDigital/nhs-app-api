package pages

open class ForbiddenError: HybridPageObject() {

    override fun assertPageHeader(headerText: String): ForbiddenError {
        super.assertPageHeader(headerText)
        return this
    }

    fun assertParagraphText(paragraphText: String): ForbiddenError {
        val message = getElement("//p[contains(text(), '$paragraphText')]")
        message.assertIsVisible()
        return this
    }

    fun assertMenuListHeader(headerText: String) : ForbiddenError{
        val menuListHeader = getElement("//h2[contains(text(),'$headerText')]")
        menuListHeader.assertIsVisible()
        return this
    }

   fun setupElement(headerText: String): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "//h2[contains(text(),'$headerText')]",
                androidLocator = null,
                page = this
        )
    }
}
