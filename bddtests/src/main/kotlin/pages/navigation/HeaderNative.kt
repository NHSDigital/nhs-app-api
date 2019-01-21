package pages.navigation

import org.junit.Assert
import org.openqa.selenium.StaleElementReferenceException
import pages.NativePageElement
import pages.NativePageObject

const val WAIT_FOR_PAGE_MS = 3000L

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

        Thread.sleep(WAIT_FOR_PAGE_MS)

        var text = ""
        var staleElement = true
        while(staleElement) {
            try {
                text = getPageTitle(expectedHeaderText).text
                staleElement = false
            }
            catch(e: StaleElementReferenceException) {
                staleElement = true
            }
        }
        Assert.assertEquals("Header is correct", expectedHeaderText, text)
    }
}
