package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.BannerObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.assertSingleElementPresent

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationConfirmationPage : OrganDonationBasePage() {

    override fun assertDisplayed() {
        waitForSpinnerToDisappear()
        title.assertIsVisible()
        amendDecisionLink.assertIsVisible()
    }

    override val titleText: String = "Your decision"

    val decisionModule by lazy { OrganDonationYourDecisionModule(this) }

    val registerBloodDonorLink = HybridPageElement(
            "//a//h2",
            page = this,
            helpfulName = "link").withText("Register to be a blood donor")

    val amendDecisionLink = getLink("Amend your decision")

    fun assertCreatedBanner() {
        BannerObject.success(this).assertVisible("We have updated your decision")
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
        BannerObject.success(this, "Decision found")
                .assertVisible("Your registration is currently being processed.")
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
