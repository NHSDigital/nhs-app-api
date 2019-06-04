package pages.nominatedPharmacy

import pages.HybridPageElement
import pages.HybridPageObject

open class PharmacyDetailComponent : HybridPageObject() {

    val pharmacyName = HybridPageElement(
            webDesktopLocator = "//div[@id='pharmacyName']",
            androidLocator = null,
            page = this
    )

    val pharmacyAddress = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacyAddress']",
            androidLocator = null,
            page = this
    )

    val pharmacyPhoneNumber = HybridPageElement(
            webDesktopLocator = "//p[@id='phoneNumber']",
            androidLocator = null,
            page = this
    )
}
