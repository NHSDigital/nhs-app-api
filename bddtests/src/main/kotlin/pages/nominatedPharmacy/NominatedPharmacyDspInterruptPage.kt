package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/dsp-interrupt")
open class NominatedPharmacyDspInterruptPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            page = this
    )

    fun isLoaded() {
        headerNative.waitForPageHeaderText(
                "If you nominate an online-only pharmacy, you may still need to register with that pharmacy separately")
    }
}