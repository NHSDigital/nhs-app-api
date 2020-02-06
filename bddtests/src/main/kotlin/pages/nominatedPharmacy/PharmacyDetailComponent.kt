package pages.nominatedPharmacy

import pages.HybridPageElement
import pages.HybridPageObject

open class PharmacyDetailComponent : HybridPageObject() {

    val pharmacyName = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacyName']",
            androidLocator = null,
            page = this
    )

    val pharmacyAddressLine1 = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacy-address-line-1']",
            androidLocator = null,
            page = this
    )

    val pharmacyAddressLine2 = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacy-address-line-2']",
            androidLocator = null,
            page = this
    )

    val pharmacyAddressLine3 = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacy-address-line-3']",
            androidLocator = null,
            page = this
    )

    val pharmacyCity = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacy-city']",
            androidLocator = null,
            page = this
    )

    val pharmacyCounty = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacy-county']",
            androidLocator = null,
            page = this
    )

    val pharmacyPostcode = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacy-postcode']",
            androidLocator = null,
            page = this
    )

    val pharmacyPhoneNumber = HybridPageElement(
            webDesktopLocator = "//p[@id='pharmacy-telephone-number']",
            androidLocator = null,
            page = this
    )

    fun isVisible(): Boolean {
        return findByXpath(pharmacyName.webDesktopLocator).isVisible ||
                findByXpath(pharmacyAddressLine1.webDesktopLocator).isVisible ||
                findByXpath(pharmacyAddressLine2.webDesktopLocator).isVisible ||
                findByXpath(pharmacyAddressLine3.webDesktopLocator).isVisible ||
                findByXpath(pharmacyCity.webDesktopLocator).isVisible ||
                findByXpath(pharmacyPostcode.webDesktopLocator).isVisible ||
                findByXpath(pharmacyPhoneNumber.webDesktopLocator).isVisible ||
                findByXpath(pharmacyCounty.webDesktopLocator).isVisible
    }
}
