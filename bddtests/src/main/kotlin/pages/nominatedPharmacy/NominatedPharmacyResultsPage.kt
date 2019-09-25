package pages.nominatedPharmacy

import models.nominatedPharmacy.PharmacySearchResult
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.WebElement
import pages.HybridPageObject
import pages.HybridPageElement
import pages.isVisible

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/results")
open class NominatedPharmacyResultsPage : HybridPageObject() {

    private val listOfPharmacies = HybridPageElement(
                webDesktopLocator = "//ul[@id='searchResults']",
                page = this
    )

    private val noResultsFoundHeader = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'No results found')]",
            page = this
    )

    fun showsNoResultsFoundHeader() {
        noResultsFoundHeader.isVisible
    }

    fun isLoaded(): Boolean {
        return listOfPharmacies.isVisible
    }

    fun getPharmacies(): List<PharmacySearchResult> {
        val results = findAll(By.cssSelector("#searchResults li"))
        val listOfPharmacies = mutableListOf<PharmacySearchResult>()
        for (result in results) {
            val pharmacyData = result.findElements<WebElement>(By.tagName("p"))
            listOfPharmacies.add(
                    PharmacySearchResult(
                        pharmacyName = pharmacyData[0].text,
                        address = pharmacyData[1].text,
                        phoneNumber = pharmacyData[2].text
            ))
        }
        return listOfPharmacies
    }

    fun selectPharmacyAtIndex(index: Int) {
        val results = findAll(By.cssSelector("#searchResults li a"))
        results[index].click()
    }
}
