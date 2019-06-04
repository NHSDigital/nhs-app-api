package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.HeaderNative
import pages.sharedElements.BannerObject

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy")
open class NominatedPharmacyPage : PharmacyDetailComponent() {

    val nominatedPharmacyLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(), 'Nominate a pharmacy')]",
            page = this
    )

    val changePharmacyLink = HybridPageElement(
                webDesktopLocator = "//a[contains(text(), 'Change my nominated pharmacy')]",
                page = this
    )

    private lateinit var headerNative: HeaderNative

    fun assertYouHaveChangedYourPharmacySuccessBannerIsVisible() {
        BannerObject.success(this).assertVisible("You changed your nominated pharmacy.")
    }

    fun isLoadedWithPharmacy() {
        headerNative.waitForPageHeaderText("My nominated pharmacy")
    }

    fun isLoadedWithNoPharmacy() {
        headerNative.waitForPageHeaderText("You have not nominated a pharmacy")
    }
}
