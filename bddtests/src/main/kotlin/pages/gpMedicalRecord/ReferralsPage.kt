package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class ReferralsPage: HybridPageObject() {

    val referralElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='referrals-card']",
        page = this
    )

    fun getReferralElements(): List<WebElementFacade> {
        return referralElements.elements
    }
}
