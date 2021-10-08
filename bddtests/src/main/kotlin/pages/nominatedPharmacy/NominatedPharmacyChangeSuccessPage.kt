package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/change-success")
open class NominatedPharmacyChangeSuccessPage : PharmacyDetailComponent() {

    private lateinit var webHeader: WebHeader

    val prescriptionsLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(), 'Go to your prescriptions')]",
            page = this
    )

    fun isLoaded() {
        webHeader.waitForPageHeaderText("You have nominated a pharmacy")
    }
}
