package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/interrupt")
open class NominatedPharmacyInterruptPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            page = this
    )

    fun isLoaded(header: String) {
        headerNative.waitForPageHeaderText(header)
    }
}
