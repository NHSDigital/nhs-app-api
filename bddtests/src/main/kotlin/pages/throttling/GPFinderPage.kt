package pages.throttling

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageObject
import pages.HybridPageElement
import pages.isVisible

class GPFinderPage : HybridPageObject() {

    companion object {
        const val validSearch = "Chesterfield"
        const val emptyInvalidSearch = ""
        const val blankInvalidSearch = "    "
    }

    private val findYourGPSurgeryHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Find your GP surgery')]",
            webMobileLocator = "//h2[contains(text(),'Find your GP surgery')]",
            androidLocator = null,
            page = this
    )

    private val loginButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), \"Login to NHS App\")]",
            webMobileLocator = "//button[contains(text(), \"Login to NHS App\")]",
            androidLocator = null,
            page = this
    )

    private val searchTermField = HybridPageElement(
            webDesktopLocator = "//*[@id='searchTextInput']",
            webMobileLocator = "//*[@id='searchTextInput']",
            androidLocator = null,
            page = this
    )

    private val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            webMobileLocator = "//button[contains(text(), 'Continue')]",
            androidLocator = null,
            page = this
    )

    private val criteriaErrorMessage = HybridPageElement(
            webDesktopLocator  = "//span[contains(text(), 'Enter postcode, town or GP surgery name')]",
            webMobileLocator = "//span[contains(text(), 'Enter the name of your GP surgery, its postcode or town')]",
            androidLocator = null,
            page = this
    )

    fun isFindYourGPSurgeryHeaderVisible(): Boolean {
        return findYourGPSurgeryHeader.isVisible
    }

    fun enterSearchTerm(searchTerm: String) {
        searchTermField.actOnTheElement {
            it.type<WebElementFacade>(searchTerm)
        }
        hideKeyboardIfOnMobile()
    }

    fun clickContinueButton() {
        continueButton.click()
    }

    fun clickLoginButton() {
        loginButton.click()
    }

    fun isSearchCriteriaErrorMessageShown(): Boolean {
        return criteriaErrorMessage.isVisible
    }
}
