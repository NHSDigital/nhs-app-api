package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/organDonation")
class OrganDonationChoicePage : HybridPageObject() {

    val organDonationTitle = HybridPageElement(
            browserLocator = "//h2",
            androidLocator = null,
            page = this,
            helpfulName = "Title"
    ).withText("Register your organ donation decision")


    val noButton = HybridPageElement(
            browserLocator = "//button[descendant::*[contains(text(),\"NO\")]]",
            page = this
    )
    val yesButton = HybridPageElement(
            browserLocator = "//button[descendant::*[contains(text(),\"YES\")]]",
            page = this
    )
}