package pages.manageCookies

import pages.HybridPageObject
import pages.sharedElements.LinksContent
import pages.sharedElements.LinksElement

class ManageCookiePolicyModule(page : HybridPageObject) : LinksElement(page, content) {
    val cookiePolicy by lazy { link(cookiePolicyLink) }

    companion object {
        private const val cookiePolicyLink = "Cookies policy"
        private var content = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//ul[@data-purpose='cookie-policy']")
                .addLink(cookiePolicyLink)
    }
}
