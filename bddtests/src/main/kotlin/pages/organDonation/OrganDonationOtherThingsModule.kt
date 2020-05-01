package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.sharedElements.LinksWithDescriptionsContent
import pages.sharedElements.LinksElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationOtherThingsModule(page :HybridPageObject) : LinksElement(page, otherThingsContent) {

    fun assertOnlyBloodLinkPresent() {
        assertReducedSetOfLinksPresent(registerBloodDonorLinkTitle)
    }

    fun withdrawDecisionLinkClick() {
        link(withdrawYourDecisionLinkTitle).click()
    }

    companion object {
        const val registerBloodDonorLinkTitle = "Register to be a blood donor"
        const val registerBloodDonorLinkDescription = "You could save lives by giving blood. " +
                "It’s simple. You can find your local centre and book an appointment via the app."

        private const val withdrawYourDecisionLinkTitle = "Withdraw your previous decision"
        private const val withdrawYourDecisionLinkDescription =
                "You can withdraw your previous decision from the Organ Donor Register at any time. " +
                        "If you do this, changes to the law around organ donation may affect you."

        private var otherThingsContent = LinksWithDescriptionsContent(
                linkBlockTitle = "Other things you can do",
                linkStyling = "h3")
                .addLink(withdrawYourDecisionLinkTitle, withdrawYourDecisionLinkDescription)
                .addLink(registerBloodDonorLinkTitle, registerBloodDonorLinkDescription)
    }
}