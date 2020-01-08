package pages.nominatedPharmacy

import models.nominatedPharmacy.OnlinePharmacySearchResult
import models.nominatedPharmacy.PharmacySearchResult
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.isVisible
import pages.assertIsNotVisible
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/results")
open class NominatedPharmacyResultsPage : HybridPageObject() {

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
                androidLocator = null,
                page = this)
    }

    private fun getPharmacyResultAddress(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$basePharmacyResultItemPath//span//p[@id='pharmacy-address-line-1']",
                androidLocator = null,
                page = this)
    }

    private fun getPharmacyResultTelephoneNumber(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$basePharmacyResultItemPath//span//p[@id='pharmacy-telephone-number']",
                androidLocator = null,
                page = this)
    }

    private fun getPharmacyResultWebsite(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$basePharmacyResultItemPath//span//p[@id='pharmacy-url']",
                androidLocator = null,
                page = this)
    }

    fun showsNoResultsFoundHeader() {
        noResultsFoundHeader.isVisible
    }

    fun showsNoResults() {
        listOfPharmacies.assertIsNotVisible()
    }

    fun isLoaded(): Boolean {
        return listOfPharmacies.isVisible
    }

    fun getPharmacies(): List<PharmacySearchResult> {
        val results = findAll(By.cssSelector("#searchResults li"))
        val listOfPharmacies = mutableListOf<PharmacySearchResult>()

        for (result in results.withIndex()) {
            setBasePharmacyResultItemPath(result.index)
            listOfPharmacies.add(
                    PharmacySearchResult(
                            pharmacyName = getPharmacyResultName().text,
                            address = getPharmacyResultAddress().text,
                            phoneNumber = getPharmacyResultTelephoneNumber().text
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
                            website = getPharmacyResultWebsite().text,
                            phoneNumber = getPharmacyResultTelephoneNumber().text
                    )
            )
        }
        return listOfPharmacies
    }

    fun selectPharmacyAtIndex(index: Int) {
        val results = findAll(By.cssSelector("#searchResults li a"))
        results[index].click()
    }
}
