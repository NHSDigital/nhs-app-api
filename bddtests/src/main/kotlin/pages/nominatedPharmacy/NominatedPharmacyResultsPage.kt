package pages.nominatedPharmacy

import models.nominatedPharmacy.PharmacySearchResult
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.navigation.WebHeader
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/results")
open class NominatedPharmacyResultsPage : HybridPageObject() {

    private lateinit var webHeader: WebHeader

    private var basePharmacyResultItemPath: String = ""

    private fun setBasePharmacyResultItemPath(index: Int) {
        basePharmacyResultItemPath = "//*[@id='pharmacy-menu-item-$index']"
    }


    private val listOfPharmacies = HybridPageElement(
            webDesktopLocator = "//ul[@id='searchResults']",
            page = this
    )

    private fun getPharmacyResultName(): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$basePharmacyResultItemPath//h2",
            page = this)
    }

    private fun getPharmacyResultAddress(index: Int): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$basePharmacyResultItemPath//p[@id='pharmacy-$index-address-line-1']",
            page = this)
    }

    private fun getPharmacyResultTelephoneNumber(index: Int): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$basePharmacyResultItemPath//p[@id='pharmacy-$index-telephone-number']",
            page = this)
    }

    fun assertIsLoaded() {
        listOfPharmacies.assertIsVisible()
    }

    fun getPharmacies(): List<PharmacySearchResult> {
        val results = findAll(By.cssSelector("#searchResults li"))
        val listOfPharmacies = mutableListOf<PharmacySearchResult>()

        for (result in results.withIndex()) {
            setBasePharmacyResultItemPath(result.index)
            listOfPharmacies.add(
                    PharmacySearchResult(
                            pharmacyName = getPharmacyResultName().text,
                            address = getPharmacyResultAddress(result.index).text,
                            phoneNumber = getPharmacyResultTelephoneNumber(result.index).text
                    )
            )
        }
        return listOfPharmacies
    }

    fun selectPharmacyAtIndex(index: Int) {
        val results = findAll(By.cssSelector("#searchResults li a"))
        results[index].click()
    }

    fun isLoaded(searchTerm: String) {
        webHeader.waitForPageHeaderText("High street pharmacies near \"$searchTerm\"")
    }
}
