package pages

open class NoOnlineAppointments: HybridPageObject() {

    private val coronaVirusInfoHeader = setupElement("If you think you might have coronavirus")
    private val gpAdviceHeader = setupElement("Ask your GP for advice")
    private val gpAdminHeader = setupElement("Additional GP services")
    private val nhs111Header = setupElement("Use NHS 111 online")

    fun assertGpAdviceMenuItem() : NoOnlineAppointments{
        gpAdviceHeader.assertIsVisible()
        return this
    }

    fun assertCoronaVirusInfoHeader() : NoOnlineAppointments {
        coronaVirusInfoHeader.assertIsVisible()
        return this
    }

    fun assertParagraphText(paragraphText: String): NoOnlineAppointments {
        val message = getElement("//p[contains(text(), \"$paragraphText\")]")
        message.assertIsVisible()
        return this
    }

    fun assertGpAdminMenuItem(): NoOnlineAppointments{
        gpAdminHeader.assertIsVisible()
        return this
    }

    fun assertNHS111Online(): NoOnlineAppointments{
        nhs111Header.assertIsVisible()
        return this
    }

    fun clickBackLink() {
        HybridPageElement(
                webDesktopLocator = "//a[@data-purpose='error']",
                page = this
        ).click()
    }

    private fun setupElement(headerText: String): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "//h2[contains(text(),'$headerText')]",
                androidLocator = null,
                page = this
        )
    }
}
