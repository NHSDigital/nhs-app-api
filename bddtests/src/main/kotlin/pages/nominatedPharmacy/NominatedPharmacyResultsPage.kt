package pages.nominatedPharmacy

import models.nominatedPharmacy.OnlinePharmacySearchResult
import models.nominatedPharmacy.PharmacySearchResult
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.assertIsNotVisible
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

    private val noResultsFoundHeader = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(), 'No results found')]",
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

    private fun getPharmacyResultWebsite(index: Int): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$basePharmacyResultItemPath//p[@id='pharmacy-$index-url']",
            page = this)
    }

    fun showsNoResultsFoundHeader() {
        noResultsFoundHeader.assertIsVisible()
    }

    fun showsNoResults() {
        listOfPharmacies.assertIsNotVisible()
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

    fun getOnlinePharmacies(): List<OnlinePharmacySearchResult> {
        val results = findAll(By.cssSelector("#searchResults li"))
        val listOfPharmacies = mutableListOf<OnlinePharmacySearchResult>()

        for (result in results.withIndex()) {
            setBasePharmacyResultItemPath(result.index)
            listOfPharmacies.add(
                    OnlinePharmacySearchResult(
                            pharmacyName = getPharmacyResultName().text,
                            website = getPharmacyResultWebsite(result.index).text,
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
