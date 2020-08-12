package pages.nominatedPharmacy

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/search")
open class SearchNominatedPharmacyPage : HybridPageObject() {

    private val searchTermField = HybridPageElement(
            webDesktopLocator = "//*[@id='searchTextInput']",
            androidLocator = null,
            page = this
    )

    val searchButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Search')]",
            page = this
    )

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Find a high street pharmacy")
    }

    fun isInvalidPostcodeErrorVisible(): Boolean {
        val message = "Enter a valid English postcode"
        return findByXpath("//li[contains(.,'$message')]").isVisible
    }

    fun isNoResultsFoundHeaderVisible(postCode: String) : Boolean {
        val message = "No results found for \"" + postCode + "\""
        return findByXpath("//H1[contains(.,'$message')]").isVisible
    }

    fun isNoResultsFoundMessageVisible(postCode: String) : Boolean {
        val message = "We could not find any results for \"" +
                postCode + "\". Make sure you enter a valid English postcode."
        return findByXpath("//p[contains(.,'$message')]").isVisible
    }

    fun isSearchAgainVisible() : Boolean {
        val message = "Search again"
        return findByXpath("//H2[contains(.,'$message')]").isVisible
    }

    fun enterSearchText(searchTerm: String) {
        searchTermField.actOnTheElement {
            it.type<WebElementFacade>(searchTerm)
        }
        hideKeyboardIfOnMobile()
    }
}
