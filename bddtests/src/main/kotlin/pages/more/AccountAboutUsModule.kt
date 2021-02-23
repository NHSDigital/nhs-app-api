package pages.more

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

class AccountAboutUsModule(page : HybridPageObject) : LinksElement(page, content) {

    companion object {
        private var content = LinksContent(linkBlockTitle = "About the NHS App")
                .addLink("Help and support")
                .addLink("Accessibility statement")
                .addLink("Open source licences")
                .addLink("Privacy policy")
                .addLink("Terms of use")
    }
}
