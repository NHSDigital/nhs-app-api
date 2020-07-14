package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/change-success")
open class OnlineNominatedPharmacyChangeSuccessPage : OnlinePharmacyDetailComponent() {

    private lateinit var headerNative: HeaderNative

    val prescriptionsLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(), 'Go to your prescriptions')]",
            page = this
    )

    fun isLoaded() {
        headerNative.waitForPageHeaderText("You have nominated a pharmacy")
    }
}
