package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.sharedElements.BannerObject
import pages.sharedElements.MenuLinksContent
import pages.sharedElements.MenuLinks

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationViewRegistrationPage : OrganDonationBasePage() {

    val shareLinkTitle ="Share that you are a donor"
    private val shareLinkDescription ="Help promote organ donation on social media by telling people you are a donor."
    val tellFamilyLinkTitle ="Tell your family"
    private val tellFamilyLinkDescription ="Use our message templates and conversation guides to tell your " +
            "family and friends you are a donor."

    val registerBloodDonorLinkTitle ="Register to be a blood donor"
    private val registerBloodDonorLinkDescription ="If you want to give more, why not sign up to give blood? " +
            "You can easily book an appointment and find your local centre via the app."

    override fun assertDisplayed() {
        waitForSpinnerToDisappear()
    }

    override val titleText: String = "Your decision"

    val decisionModule by lazy { OrganDonationYourDecisionModule(this) }

    val amendDecisionLink = getLink("Amend your decision")

    private var nextStepsContent = MenuLinksContent(
            title = "Next steps",
            links = arrayOf(Pair(shareLinkTitle, shareLinkDescription),
                    Pair(tellFamilyLinkTitle, tellFamilyLinkDescription)),
            textOverride = "Please inform your family about your decision.")

    val nextSteps =  MenuLinks(this, nextStepsContent)

    private var otherThingsContent = MenuLinksContent(
            title= "Other things you can do",
            links =  arrayOf(Pair(registerBloodDonorLinkTitle, registerBloodDonorLinkDescription)))

    val otherThings =  MenuLinks(this, otherThingsContent)

    fun assertCreatedBanner() {
        BannerObject.success(this).assertVisible("We have updated your decision")
        title.assertIsVisible()
        amendDecisionLink.assertIsVisible()
    }

    fun assertDecisionSubmitted() {
        BannerObject.success(this, "Decision submitted")
                .assertVisible("We have successfully received your organ donation decision.")
        assertText("What happens next",
                "We will process your decision and you will then be able to view and amend this via the NHS App. " +
                        "This may take up to 4 days. " +
                        "Remember to let your family know your decision about organ donation.")
    }

    fun assertDecisionFound() {
        waitForSpinnerToDisappear()
        waitForElement{BannerObject.success(this, "Decision found")
                .assertVisible("Your registration is currently being processed.")}
        assertText("We are still processing your registration",
                "Please check back in 2 days. " +
                        "You’ll then be able to view and amend your decision via the NHS App. " +
                        "Remember to let your family know your decision about organ donation.")
    }

    private fun assertText(header: String, text: String) {
        HybridPageElement(
                " //div[strong[contains(text(),'$header')]]/p[contains(text(),'$text')]",
                page = this).assertSingleElementPresent()

    }
}