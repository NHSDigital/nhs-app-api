package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/confirm")
open class ConfirmNominatedPharmacyPage : PharmacyDetailComponent() {

    val confirmButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Confirm')]",
            page = this
    )

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Confirm my nominated pharmacy")
    }
}
