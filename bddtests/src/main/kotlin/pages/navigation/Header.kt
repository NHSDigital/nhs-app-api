package pages.navigation

import pages.HybridPageObject
import pages.HybridPageElement

class Header: HybridPageObject(Companion.PageType.NATIVE) {

    val homeIcon = HybridPageElement(
            androidLocator = "//android.widget.ImageView[contains(@resource-id,'nhsOnlineLogoIcon')]",
            browserLocator = "//*[@id='nhs_logo']",
            page = this
    )
    val helpIcon = HybridPageElement(
            androidLocator = "//android.widget.ImageView[contains(@resource-id,'helpIcon')]",
            browserLocator = "//a[@id='help_icon']/*[name()='svg']",
            page = this
    )
    val accountIcon = HybridPageElement(
            androidLocator = "//android.widget.ImageView[contains(@resource-id,'myAccountIcon')]",
            browserLocator = "//a[@href='/account']/*[name()='svg']",
            page = this
    )
    val pageTitle = HybridPageElement(
            androidLocator = "//*[contains(@resource-id, 'header_text_view')]",
            browserLocator = "//header/h1",
            page = this
    )

    fun isVisible(title: String): Boolean {
        val logoIsVisible = homeIcon.element.isVisible
        val accountIsVisible = accountIcon.element.isVisible
        val helpIsVisible = helpIcon.element.isVisible
        val headingIsVisible = pageTitle.withText(title).element.isVisible

        return logoIsVisible && accountIsVisible && headingIsVisible && helpIsVisible
    }

    fun clickMyAccount() {
        accountIcon.element.click()
    }

    fun clickHelp() {
        helpIcon.element.click()
    }

    fun clickHome() {
        homeIcon.element.click()
    }
}
