package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.sharedElements.MenuLinksContent
import pages.sharedElements.MenuLinks

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationNextStepsModule(page :HybridPageObject) : MenuLinks(page, nextStepsContent) {

    fun assertOnlyTellFamilyLinkPresent() {
        assertLinksPresent(tellFamilyLinkTitle)
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
                links = arrayOf(Pair(tellFamilyLinkTitle, tellFamilyLinkDescription),
                        Pair(shareLinkTitle, shareLinkDescription)),
                linkStyling = "h3")
    }
}