package pages.loggedOut

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.withNormalisedText

@DefaultUrl("http://web.local.bitraft.io:3000/login")
class CookieBanner : HybridPageObject() {

    private val cookieBannerXpath = "//div[@data-purpose='cookie-banner']"

    val cookieBannerText1 = HybridPageElement(
            webDesktopLocator = "$cookieBannerXpath/p",
            page = this,
            timeToWaitForElement = 1
    ).withNormalisedText(
            "We've put some small files called cookies on your device. " +
            "These are the strictly necessary cookies needed to make the NHS App work.")

    val cookieBannerText2 = HybridPageElement(
            webDesktopLocator = "$cookieBannerXpath//p[contains(text(),'" +
                    "We will not use any other cookies unless you choose to turn them on, as described in our" +
                    "')]",
            page = this,
            timeToWaitForElement = 1
    )

    val cookiesInformationLink = HybridPageElement(
            webDesktopLocator = "$cookieBannerXpath//p//a[normalize-space(text())='cookies policy']",
            page = this,
            timeToWaitForElement = 1
    )

    val cookieBannerClose = HybridPageElement(
            webDesktopLocator = "$cookieBannerXpath//button",
            page = this,
            timeToWaitForElement = 1
    )
}
