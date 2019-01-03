package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
class OrganDonationNewRegistrationPage : HybridPageObject() {

    private val organDonationTitle = HybridPageElement(
            browserLocator = "//p[contains(text(),'Register your organ donation decision')]",
            androidLocator = null,
            page = this
    )

    fun isSubTitleVisible(): Boolean {
        return organDonationTitle.element.isVisible
    }
}