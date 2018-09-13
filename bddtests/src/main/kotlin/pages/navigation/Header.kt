package pages.navigation

import org.junit.Assert
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

    fun assertIsVisible(title: String) {

        Assert.assertTrue("Expected logo to be visible", homeIcon.element.isVisible)
        Assert.assertTrue("Expected account icon to be visible", accountIcon.element.isVisible)
        Assert.assertTrue("Expected help icon to be visible", helpIcon.element.isVisible)
        waitForPageHeaderText(title)
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
