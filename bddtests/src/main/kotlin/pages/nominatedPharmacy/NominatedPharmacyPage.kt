package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.navigation.HeaderNative
import pages.sharedElements.BannerObject

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy")
open class NominatedPharmacyPage : PharmacyDetailComponent() {

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

    private lateinit var headerNative: HeaderNative

    fun assertYouHaveChangedYourPharmacySuccessBannerIsVisible() {
        BannerObject.success(this).assertVisible("You changed your nominated pharmacy.")
    }

    fun assertYouHaveChosenYourNominatedPharmacyBannerIsVisible() {
        BannerObject.success(this).assertVisible("You've chosen your nominated pharmacy.")
    }

    fun isLoadedWithPharmacy() {
        headerNative.waitForPageHeaderText("Your nominated pharmacy")
    }

    fun isLoadedWithDispensingPractiseHeader() {
        headerNative.waitForPageHeaderText("Your dispensing practice")
    }

    fun isLoadedWithNoPharmacy() {
        headerNative.waitForPageHeaderText("Nominate your pharmacy")
    }
}
