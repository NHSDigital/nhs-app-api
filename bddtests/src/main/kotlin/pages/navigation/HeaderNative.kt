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
    private val moreIcon = NativePageElement(
            androidLocator = getAndroidIconLocator("moreIcon"),
            webDesktopLocator = "//a[@href='more']",
            webMobileLocator = "//a[@href='more']",
            iOSAccessID = "More",
            page = this
    )

    private val moreLink = NativePageElement(
            androidLocator = getAndroidIconLocator("moreIcon"),
            webDesktopLocator = "//a[@id='more-link']",
            webMobileLocator = "//a[@id='more-link']",
            iOSAccessID = "More",
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
                moreIcon.elements.count() == 1 || moreLink.elements.count() == 1)

        val optionManager = OptionManager.instance()
        when {
            optionManager.isEnabled(DeviceWebDesktop::class) ->
                 helpIcon.assertIsDisplayed("Expected help icon to be visible")
        }

        waitForPageHeaderText(title)
    }

    fun clickMore() {
        if ( moreIcon.isDisplayed() ) {
            moreIcon.click()
        }
        else {
            moreLink.click()
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
