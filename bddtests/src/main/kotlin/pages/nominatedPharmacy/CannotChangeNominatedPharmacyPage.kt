package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/search")
open class CannotChangeNominatedPharmacyPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val changePharmacyInstruction = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'How to change your nominated pharmacy')]",
            page = this
    )

    fun isLoaded() {
        headerNative.waitForPageHeaderText("You cannot change your nominated pharmacy with the NHS App")
    }
}