package pages.account

import pages.HybridPageObject
import pages.sharedElements.LinksContent
import pages.sharedElements.LinksElement

class AccountCookieLinkModule(page : HybridPageObject) : LinksElement(page, content) {
    val cookie by lazy { link(cookiesLink) }

    companion object {
        private const val cookiesLink = "Cookies"
        private var content = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//ul[@data-purpose='cookie-menu']")
                .addLink(cookiesLink)
    }
}