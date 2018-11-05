package pages.navigation

import org.junit.Assert
import org.openqa.selenium.TimeoutException
import pages.NativePageElement
import pages.NativePageObject

class HeaderNative : NativePageObject() {

    private fun getAndroidIconLocator(id: String): String {
        return  "//android.widget.ImageView[contains(@resource-id,'${id}')]"
    }

    val homeIcon = NativePageElement(
            androidLocator = getAndroidIconLocator("nhsOnlineLogoIcon"),
            browserLocator = "//*[@id='nhs_logo']",
            iOSAccessID = "NHS App Home",
            page = this
    )
    val helpIcon = NativePageElement(
            androidLocator = getAndroidIconLocator("helpIcon"),
            browserLocator = "//a[@id='help_icon']/*[name()='svg']",
            iOSAccessID = "Help and support",
            page = this
    )
    val accountIcon = NativePageElement(
            androidLocator = getAndroidIconLocator("myAccountIcon"),
            browserLocator = "//a[@href='/account']/*[name()='svg']",
            iOSAccessID = "My account",
            page = this
    )

    fun getPageTitle(title: String) : NativePageElement {
        return  NativePageElement(
                androidLocator = "//*[contains(@resource-id, 'header_text_view')]",
                browserLocator = "//header/h1",
                iOSAccessID = title,
                page = this
        )
    }

    fun assertIsVisible(title: String) {
        Assert.assertTrue("Expected logo to be visible", homeIcon.isDisplayed())
        Assert.assertTrue("Expected account icon to be visible", accountIcon.isDisplayed())
        Assert.assertTrue("Expected help icon to be visible", helpIcon.isDisplayed())
        waitForPageHeaderText(title)
    }

    fun clickMyAccount() {
        accountIcon.click()
    }

    fun clickHelp() {
        helpIcon.click()
    }

    fun clickHome() {
        homeIcon.click()
    }

    fun waitForPageHeaderText(expectedHeaderText: String) {
        Assert.assertEquals(
                "Header is incorrect",
                true,
                try {
                    waitFor {
                        checkPageHeaderText(expectedHeaderText) == true
                    }
                    true
                } catch (e: TimeoutException) {
                    false
                }
        )
    }
    private fun checkPageHeaderText(title: String): Boolean {
            return getPageTitle(title).text == title
    }
}
