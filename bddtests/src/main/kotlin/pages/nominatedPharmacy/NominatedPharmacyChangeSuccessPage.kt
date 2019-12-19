package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/change-success")
open class NominatedPharmacyChangeSuccessPage : PharmacyDetailComponent() {

    private lateinit var headerNative: HeaderNative

    val prescriptionsLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(), 'Go to your repeat prescriptions')]",
            page = this
    )

    fun isLoaded(pharmacyName: String) {
        headerNative.waitForPageHeaderText("Your nominated pharmacy is " + pharmacyName)
    }
}