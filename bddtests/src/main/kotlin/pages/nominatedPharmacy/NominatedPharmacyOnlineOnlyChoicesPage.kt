package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/online-only-choices")
open class NominatedPharmacyOnlineOnlyChoicesPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val yesRadioButton = HybridPageElement(
            webDesktopLocator = "//input[@id='radioButton-true']",
            webMobileLocator = "//input[@id='radioButton-true']",
            androidLocator = null,
            page = this
    )

    val noRadioButton = HybridPageElement(
            webDesktopLocator = "//input[@id='radioButton-false']",
            webMobileLocator = "//input[@id='radioButton-false']",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            page = this
    )

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Is there a specific online-only pharmacy that you want to use?")
    }

}