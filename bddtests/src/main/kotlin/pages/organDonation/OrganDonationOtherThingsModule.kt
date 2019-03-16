package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.sharedElements.MenuLinksContent
import pages.sharedElements.MenuLinks

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationOtherThingsModule(page :HybridPageObject) : MenuLinks(page, otherThingsContent) {

    fun assertOnlyBloodLinkPresent(){
        assertPresentWithLinks(arrayOf(Pair(registerBloodDonorLinkTitle, registerBloodDonorLinkDescription)))
    }

    fun registerBloodDonorLinkClick() {
        link(registerBloodDonorLinkTitle).click()
    }

    fun withdrawDecisionLinkClick() {
        link(withdrawYourDecisionLinkTitle).click()
    }

    companion object {
        const val registerBloodDonorLinkTitle = "Register to be a blood donor"
        const val registerBloodDonorLinkDescription = "If you want to give more, " +
                "why not sign up to give blood? " +
                "You can easily book an appointment and find your local centre via the app."

        private const val withdrawYourDecisionLinkTitle ="Withdraw your decision"
        private const val withdrawYourDecisionLinkDescription =
                "Remove an existing registration from the Organ Donor Register. " +
                        "There will be no recorded decision for you about organ donation."

        private var otherThingsContent = MenuLinksContent(
                title = "Other things you can do",
                links = arrayOf((Pair(withdrawYourDecisionLinkTitle, withdrawYourDecisionLinkDescription)),
                        Pair(registerBloodDonorLinkTitle, registerBloodDonorLinkDescription)))
    }
}