package pages.account

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

class AccountAboutUsModule(page : HybridPageObject) : LinksElement(page, content) {

    companion object {
        private var content = LinksContent(linkBlockTitle = "About us")
                .addLink("Terms of use")
                .addLink("Privacy policy")
                .addLink("Cookies policy")
                .addLink("Open source licences")
                .addLink("Help and support")
                .addLink("Accessibility statement")
    }
}