package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/interrupt")
open class NominatedPharmacyInterruptPage : HybridPageObject() {

    private lateinit var webHeader: WebHeader

    val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            page = this
    )

    fun isLoaded(header: String) {
        webHeader.waitForPageHeaderText(header)
    }
}
