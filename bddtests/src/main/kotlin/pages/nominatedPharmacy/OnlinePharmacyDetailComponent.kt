package pages.nominatedPharmacy

import pages.HybridPageElement
import pages.HybridPageObject

open class OnlinePharmacyDetailComponent : HybridPageObject() {

    val pharmacyName = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacyName']",
            androidLocator = null,
            page = this
    )

    val pharmacyUrl = HybridPageElement(
            webDesktopLocator = "//p[@id='url']",
            androidLocator = null,
            page = this
    )

    val pharmacyPhoneNumber = HybridPageElement(
            webDesktopLocator = "//p[@id='online-only-phone-number']",
            androidLocator = null,
            page = this
    )

    fun isVisible(): Boolean {
        return findByXpath(pharmacyName.webDesktopLocator).isVisible ||
                findByXpath(pharmacyUrl.webDesktopLocator).isVisible ||
                findByXpath(pharmacyPhoneNumber.webDesktopLocator).isVisible
    }
}
