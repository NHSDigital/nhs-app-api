package pages.navigation

import org.junit.Assert
import org.openqa.selenium.StaleElementReferenceException
import pages.NativePageElement
import pages.NativePageObject
import webdrivers.options.OptionManager
import webdrivers.options.device.DeviceWebDesktop

private const val WAIT_FOR_PAGE_MS = 3000L
private const val HEADER_RETRIES = 20

class HeaderNative : NativePageObject() {

    private fun getAndroidIconLocator(id: String): String {
        return "//android.widget.ImageView[contains(@resource-id,'${id}')]"
    }

    private val homeIcon = NativePageElement(
            androidLocator = getAndroidIconLocator("homeLogoIcon"),
            webDesktopLocator = "//*[@id='nhs_logo']",
            webMobileLocator = "//*[@id='nhs_logo']",
            iOSAccessID = "NHS App Home",
            page = this
    )
    private val helpIcon = NativePageElement(
            androidLocator = getAndroidIconLocator("helpIcon"),
            webDesktopLocator = "//a[@id='help_icon']",
            webMobileLocator = "//a[@id='help_icon']",
            iOSAccessID = "Help and support",
            page = this
    )
    private val accountIcon = NativePageElement(
            androidLocator = getAndroidIconLocator("myAccountIcon"),
            webDesktopLocator = "//a[@href='account']",
            webMobileLocator = "//a[@href='account']",
            iOSAccessID = "My account",
            page = this
    )

    private val accountLink = NativePageElement(
            androidLocator = getAndroidIconLocator("myAccountIcon"),
            webDesktopLocator = "//a[@id='account-link']",
            webMobileLocator = "//a[@id='account-link']",
            iOSAccessID = "My account",
            page = this
    )

    fun getPageTitle(title: String): NativePageElement {
        return NativePageElement(
                androidLocator = "//*[contains(@resource-id, 'header_text_view')]",
                webDesktopLocator = "//h1",
                webMobileLocator = "//h1",
                iOSAccessID = title,
                page = this
        )
    }

    fun assertIsVisible(title: String) {
        homeIcon.assertIsDisplayed("Expected logo to be visible")

        Assert.assertTrue("Expected account icon to be visible",
                accountIcon.elements.count() == 1 || accountLink.elements.count() == 1)

        val optionManager = OptionManager.instance()
        when {
            optionManager.isEnabled(DeviceWebDesktop::class) ->
                 helpIcon.assertIsDisplayed("Expected help icon to be visible")
        }

        waitForPageHeaderText(title)
    }

    fun clickMyAccount() {
        if ( accountIcon.isDisplayed() ) {
            accountIcon.click()
        }
        else {
            accountLink.click()
        }
    }

    fun clickHelp() {
        helpIcon.click()
    }

    fun clickHome() {
        homeIcon.click()
    }

    fun waitForPageHeaderText(expectedHeaderText: String) {

        Thread.sleep(WAIT_FOR_PAGE_MS)
        var retryAssertionsRemaining = HEADER_RETRIES
        while (retryAssertionsRemaining > 0) {
            try {
                val title = getPageTitle(expectedHeaderText)
                Assert.assertEquals("Header Expected", expectedHeaderText, title.text)
                break
            } catch (e: StaleElementReferenceException) {
                Thread.sleep(WAIT_FOR_PAGE_MS)
            } catch (e: AssertionError) {
                retryAssertionsRemaining--
                if (retryAssertionsRemaining == 0) {
                    throw(e)
                }
                Thread.sleep(WAIT_FOR_PAGE_MS)
            }
        }
    }
}
