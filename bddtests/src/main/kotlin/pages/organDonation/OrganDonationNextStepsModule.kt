package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.sharedElements.MenuLinksContent
import pages.sharedElements.MenuLinks

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationNextStepsModule(page :HybridPageObject) : MenuLinks(page, nextStepsContent) {

    fun shareLinkClick() {
        link(shareLinkTitle).click()
    }

    fun tellFamilyLinkClick() {
        link(tellFamilyLinkTitle).click()
    }

    companion object {

        private const val shareLinkTitle ="Share that you are a donor"
        private const val shareLinkDescription ="Help promote organ donation on social media " +
                "by telling people you are a donor."
        private const val tellFamilyLinkTitle ="Tell your family and friends"
        private const val tellFamilyLinkDescription ="Use our message templates and " +
                "conversation guides to tell your family and friends you are a donor."

        private var nextStepsContent = MenuLinksContent(
                title = "Next steps",
                links = arrayOf(Pair(shareLinkTitle, shareLinkDescription),
                        Pair(tellFamilyLinkTitle, tellFamilyLinkDescription)),
                textOverride = "Please inform your family about your decision.",
                linkStyling = "h3")
    }
}