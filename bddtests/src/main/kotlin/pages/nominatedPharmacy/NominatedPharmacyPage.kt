package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy")
open class NominatedPharmacyPage : PharmacyDetailComponent() {

    private lateinit var webHeader: WebHeader

    val changePharmacyLink = HybridPageElement(
                webDesktopLocator = "//button[contains(text(), 'Change your nominated pharmacy')]",
                page = this
    )

    val cannotChangeDispensingPractiseInformationLine1 = HybridPageElement(
            webDesktopLocator = "//*[@id='warning-text-1']",
            page = this
    )

    val cannotChangeDispensingPractiseInformationLine2 = HybridPageElement(
            webDesktopLocator = "//*[@id='warning-text-2']",
            page = this
    )

    fun isLoadedWithPharmacy() {
        webHeader.waitForPageHeaderText("Your nominated pharmacy")
    }

    fun isLoadedWithDispensingPractiseHeader() {
        webHeader.waitForPageHeaderText("Your dispensing practice")
    }
}
