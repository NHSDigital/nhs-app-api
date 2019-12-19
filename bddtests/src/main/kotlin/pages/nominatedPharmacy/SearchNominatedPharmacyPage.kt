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
        headerNative.waitForPageHeaderText("Change your nominated pharmacy")
    }

    fun enterSearchText(searchTerm: String) {
        searchTermField.actOnTheElement {
            it.type<WebElementFacade>(searchTerm)
        }
        hideKeyboardIfOnMobile()
    }
}