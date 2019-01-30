package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.BannerObject
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationConfirmationPage : OrganDonationBasePage() {

    override fun assertDisplayed() {
        title.assertIsVisible()
    }

    override val titleText: String = "Your decision"

    val decisionModule by lazy { OrganDonationYourDecisionModule(this) }

    val registerBloodDonorLink = HybridPageElement(
            "//a//h2",
            page = this,
            helpfulName = "link").withText("Register to be a blood donor")

    fun assertSuccessBanner() {
        BannerObject.success(this).assertVisible("We have updated your decision")
    }
}