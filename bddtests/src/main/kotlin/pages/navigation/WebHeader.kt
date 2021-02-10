package pages.navigation

import pages.HybridPageElement
import pages.HybridPageObject

open class WebHeader : HybridPageObject() {

    var pageHeaders: Map<String, String> = mapOf(
            Pair("Advice", "Advice"),
            Pair("Appointments", "Your appointments"),
            Pair("Prescriptions", "Prescriptions"),
            Pair("Your medical record", "Your medical record"),
            Pair("Account", "My account"),
            Pair("Home", "Home")
    )

    val advicePageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='advicePageLink']",
            page = this
    )

    val appointmentsPageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='appointmentsPageLink']",
            page = this
    )


    val yourHealthPageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='myRecordPageLink']",
            page = this
    )


    val prescriptionsPageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='prescriptionsPageLink']",
            page = this
    )


    val logoutLink = HybridPageElement(
            webDesktopLocator = "//a[normalize-space(text())='Log out']",
            page = this
    )

    val accountLink = HybridPageElement(
            webDesktopLocator = "//a[normalize-space(text())='Settings']",
            page = this
    )

    fun getPageTitle(): HybridPageElement {
        val headerXPath = "//h1"
        return HybridPageElement(
                webDesktopLocator = headerXPath,
                webMobileLocator = headerXPath,
                page = this
        )
    }

    fun getHtmlElement(element: String): HybridPageElement {
        val headerXPath = "//${element}"
        return HybridPageElement(
                webDesktopLocator = headerXPath,
                webMobileLocator = headerXPath,
                page = this
        )
    }

    fun clickAdvicePageLink() {
        advicePageLink.click()
    }

    fun clickAppointmentsPageLink() {
        appointmentsPageLink.click()
    }

    fun clickYourHealthPageLink() {
        yourHealthPageLink.click()
    }

    fun clickPrescriptionsPageLink() {
        prescriptionsPageLink.click()
    }

    fun clickLogout() {
        logoutLink.click()
    }

    fun clickAccount() {
        accountLink.click()
    }

    fun isPageTitleCorrect(page: String) {
        getPageTitle().withText(pageHeaders.get(page)!!)
    }
}
