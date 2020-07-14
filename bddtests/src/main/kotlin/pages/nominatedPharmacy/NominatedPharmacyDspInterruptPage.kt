package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/dsp-interrupt")
open class NominatedPharmacyDspInterruptPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val prescriptionsHomeLink = HybridPageElement(
            webDesktopLocator = "//*[@id='prescriptions-home-link']/a",
            page = this
    )

    fun isLoaded() {
        headerNative.waitForPageHeaderText(
                "Register with the online-only pharmacy directly")
    }
}
