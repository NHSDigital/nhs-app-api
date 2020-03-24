package pages.manageCookies

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/account/cookies")
class ManageCookiesPage : HybridPageObject() {

    val pageBody = HybridPageElement(
            webDesktopLocator = "/html/body",
            iOSLocator = "/html/body",
            androidLocator = "/html/body",
            page = this
    )
    val cookiesLink = HybridPageElement(
            webDesktopLocator = "//*[@id=\"'cookies'\"]",
            iOSLocator = "//*[@id=\"'cookies'\"]",
            androidLocator = "//*[@id=\"'cookies'\"]",
            page = this
    )
    private val cookiesPolicy = ManageCookiePolicyModule(this)

    val cookieToggle = ToggleElement(this, "Allow optional analytic cookies", "allow_cookies")

    fun assertDisplayed() {
        cookiesPolicy.cookiePolicy.assertSingleElementPresent()
        cookieToggle.assertIsVisible()
    }

}
