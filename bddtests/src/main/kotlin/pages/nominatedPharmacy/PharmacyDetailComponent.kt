package pages.nominatedPharmacy

import pages.HybridPageElement
import pages.HybridPageObject

open class PharmacyDetailComponent : HybridPageObject() {

    val pharmacyName = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacyName']",
        page = this
    )

    val pharmacyAddressLine1 = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacy-0-address-line-1']",
        page = this
    )

    val pharmacyAddressLine2 = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacy-0-address-line-2']",
        page = this
    )

    val pharmacyAddressLine3 = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacy-0-address-line-3']",
        page = this
    )

    val pharmacyCity = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacy-0-city']",
        page = this
    )

    val pharmacyCounty = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacy-0-county']",
        page = this
    )

    val pharmacyPostcode = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacy-0-postcode']",
        page = this
    )

    val pharmacyPhoneNumber = HybridPageElement(
        webDesktopLocator = "//p[@id='pharmacy-0-telephone-number']",
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
