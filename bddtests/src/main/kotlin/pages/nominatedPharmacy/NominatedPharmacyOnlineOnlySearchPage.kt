package pages.nominatedPharmacy

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/online-only-search")
open class NominatedPharmacyOnlineOnlySearchPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("What is the name of the online-only pharmacy?")
    }

    private val searchField = HybridPageElement(
            webDesktopLocator = "//*[@id='searchTextInput']",
            webMobileLocator = "//*[@id='searchTextInput']",
            androidLocator = null,
            page = this
    )

    public val searchButton = HybridPageElement(
            webDesktopLocator = "//*[@id='search-button']",
            androidLocator = null,
            page = this
    )

    fun isNoResultsFoundHeaderVisible(searchTerm: String) : Boolean {
        val message = "No results found for \"" + searchTerm + "\""
        return findByXpath("//H1[contains(.,'$message')]").isVisible
    }

    fun isErrorMessageTextVisible() : Boolean {
        return findByXpath("//li[contains(.,'Enter the name of the online-only pharmacy')]").isVisible
    }

    fun isNoResultsFoundMessageVisible(searchTerm: String) : Boolean {
        val message = "We could not find any results for \"" + searchTerm +
                "\". Make sure you enter the pharmacy name correctly."
        return findByXpath("//p[contains(.,'$message')]").isVisible
    }

    fun isSearchAgainH2Visible() : Boolean {
        return findByXpath("//H2[contains(.,'Search again')]").isVisible
    }

    fun enterTextToSearchField(searchQuery: String) {
        searchField.actOnTheElement {
            it.type<WebElementFacade>(searchQuery)
        }
        hideKeyboardIfOnMobile()
    }
}