package pages.navigation

import pages.HybridPageObject
import pages.HybridPageElement


open class WebHeader : HybridPageObject() {

    var pageHeaders: Map<String, String> = mapOf(
            Pair("Symptoms", "Check my symptoms"),
            Pair("Appointments", "My appointments"),
            Pair("Repeat prescriptions", "My repeat prescriptions"),
            Pair("My medical record", "My medical record"),
            Pair("More", "More"),
            Pair("Account", "My account"),
            Pair("Home", "Home")
    )

    val symptomsPageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='symptomsPageLink']",
            page = this
    )

    val appointmentsPageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='appointmentsPageLink']",
            page = this
    )


    val myRecordPageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='myRecordPageLink']",
            page = this
    )


    val prescriptionsPageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='prescriptionsPageLink']",
            page = this
    )


    val morePageLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='morePageLink']",
            page = this
    )


    val logoutLink = HybridPageElement(
            webDesktopLocator = "//a[normalize-space(text())='Log out']",
            page = this
    )

    val accountLink = HybridPageElement(
            webDesktopLocator = "//a[normalize-space(text())='Account']",
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

    fun clickSymptomsPageLink() {
        symptomsPageLink.click()
    }

    fun clickAppointmentsPageLink() {
        appointmentsPageLink.click()
    }

    fun clickMyRecordPageLink() {
        myRecordPageLink.click()
    }

    fun clickPrescriptionsPageLink() {
        prescriptionsPageLink.click()
    }

    fun clickMorePageLink() {
        morePageLink.click()
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
