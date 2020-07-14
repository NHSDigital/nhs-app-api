package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.sharedElements.LinksWithDescriptionsContent
import pages.sharedElements.LinksElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationNextStepsModule(page :HybridPageObject) : LinksElement(page, nextStepsContent) {

    fun assertOnlyTellFamilyLinkPresent() {
        assertReducedSetOfLinksPresent(tellFamilyLinkTitle)
    }

    companion object {

        private const val shareLinkTitle ="Share that you are a donor"
        private const val shareLinkDescription ="Help promote organ donation on social media " +
                "by telling people you are a donor."
        private const val tellFamilyLinkTitle ="Tell your family and friends"
        private const val tellFamilyLinkDescription ="Do your family and friends know what you want? " +
                "Help them to support your decision by talking about it."

        private var nextStepsContent = LinksWithDescriptionsContent(
                linkBlockTitle = "Next steps",
                linkStyling = "h3")
                .addLink(tellFamilyLinkTitle, tellFamilyLinkDescription)
                .addLink(shareLinkTitle, shareLinkDescription)
    }
}
