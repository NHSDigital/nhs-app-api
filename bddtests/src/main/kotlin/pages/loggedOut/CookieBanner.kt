package pages.loggedOut

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/login")
class CookieBanner : HybridPageObject() {

    private val cookieBannerXpath = "//div[@data-purpose='cookie-banner']"

    val cookieBanner = HybridPageElement(
            webDesktopLocator = cookieBannerXpath,
            page = this,
            timeToWaitForElement = 1
    )

    val cookieBannerText = HybridPageElement(
            webDesktopLocator = "$cookieBannerXpath//span[normalize-space(text())='" +
                    "The NHS website uses cookies to improve your on-site experience." +
                    "']",
            page = this,
            timeToWaitForElement = 1
    )

    val cookiesInformationLink = HybridPageElement(
            webDesktopLocator = "$cookieBannerXpath//a[normalize-space(text())='Find out more about cookies']",
            page = this,
            timeToWaitForElement = 1
    )

    val cookieBannerClose = HybridPageElement(
            webDesktopLocator = "$cookieBannerXpath//button",
            page = this,
            timeToWaitForElement = 1
    )
}
