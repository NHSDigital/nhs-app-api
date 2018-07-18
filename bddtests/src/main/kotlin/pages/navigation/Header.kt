package pages.navigation

import pages.HybridPageObject
import pages.HybridPageElement

class Header: HybridPageObject(Companion.PageType.NATIVE) {

    val homeIcon = HybridPageElement(
            androidLocator = "//android.widget.ImageView[contains(@resource-id,'nhsOnlineLogoIcon')]",
            browserLocator = "//*[@id='nhs_logo']",
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
        val headingIsVisible = pageTitle.withText(title).element.isVisible

        return logoIsVisible && accountIsVisible && headingIsVisible
    }

    fun clickMyAccount() {
        accountIcon.element.click()
    }
}
