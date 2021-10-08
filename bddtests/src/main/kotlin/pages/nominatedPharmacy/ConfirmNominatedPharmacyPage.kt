package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/confirm")
open class ConfirmNominatedPharmacyPage : PharmacyDetailComponent() {

    val confirmButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Confirm')]",
            page = this
    )

    private lateinit var webHeader: WebHeader

    fun isLoaded() {
        webHeader.waitForPageHeaderText("Check your nominated pharmacy details")
    }
}
