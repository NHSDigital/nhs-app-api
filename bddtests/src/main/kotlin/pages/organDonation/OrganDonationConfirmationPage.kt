package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.BannerObject

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationConfirmationPage : HybridPageObject() {

    val title = HybridPageElement(
            "//h2",
            page = this,
            helpfulName = "header").withText("Your decision")

    private val decision = HybridPageElement(
            "//span",
            page = this,
            helpfulName = "header")

    fun assertDecisionIsNo() {
        decision.withText("No, I do not want to donate my organs").assertSingleElementPresent()
    }

    fun assertDecisionIsYes() {
        decision.withText("Yes, I do want to donate my organs").assertSingleElementPresent()
    }

    fun assertSuccessBanner() {
        BannerObject.success(this).assertVisible("We have updated your decision")
    }
}