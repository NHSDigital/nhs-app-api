package pages.legalAndCookies

import pages.HybridPageObject
import pages.sharedElements.LinksContent
import pages.sharedElements.LinksElement

class LegalAndCookiesManageCookiePolicyModule(page : HybridPageObject) : LinksElement(page, content) {
    val cookiePolicy by lazy { link(cookiePolicyLink) }

    companion object {
        private const val cookiePolicyLink = "Cookies policy"
        private var content = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//ul[@data-purpose='cookie-policy']")
                .addLink(cookiePolicyLink)
    }
}
