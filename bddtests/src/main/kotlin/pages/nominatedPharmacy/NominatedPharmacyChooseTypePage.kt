package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/choose-type")
open class NominatedPharmacyChooseTypePage : HybridPageObject() {

    private lateinit var webHeader: WebHeader

    val highStreetPharmacyRadioButton = HybridPageElement(
        webDesktopLocator = "//input[@id='radioButton-highStreet']",
        page = this
    )

    val onlinePharmacyRadioButton = HybridPageElement(
        webDesktopLocator = "//input[@id='radioButton-online']",
        page = this
    )

    val continueButton = HybridPageElement(
        webDesktopLocator = "//button[contains(text(), 'Continue')]",
        page = this
    )

    fun isLoaded() {
        webHeader.waitForPageHeaderText("Choose a type of pharmacy to search for")
    }

}
