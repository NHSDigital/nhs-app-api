package pages.throttling

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageObject
import pages.HybridPageElement
import pages.myrecord.SHRUB_ANIMATION_DURATION_MILLIS

class GPFinderPage : HybridPageObject() {

    companion object {
        const val validSearch = "Chesterfield"
        const val emptyInvalidSearch = ""
        const val blankInvalidSearch = "    "
    }

    private val findYourGPSurgeryHeader = HybridPageElement(
            browserLocator = "//h4[contains(text(),'Find your GP surgery')]",
            androidLocator = null,
            page = this
    )

    private val alreadyUsingAppLink = HybridPageElement(
            browserLocator = "//a[contains(text(), \"I'm already using the NHS App\")]",
            androidLocator = null,
            page = this
    )

    private val searchTermField = HybridPageElement(
            browserLocator = "//*[@id='searchTextInput']",
            androidLocator = null,
            page = this
    )

    private val continueButton = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Continue')]",
            androidLocator = null,
            page = this
    )

    private val criteriaErrorMessage = HybridPageElement(
            browserLocator = "//span[contains(text(), 'Enter postcode, town or GP surgery name')]",
            androidLocator = null,
            page = this
    )

    fun isFindYourGPSurgeryHeaderVisible(): Boolean {
        return findYourGPSurgeryHeader.element.isVisible
    }

    fun enterSearchTerm(searchTerm: String) {
        searchTermField.element.type<WebElementFacade>(searchTerm)
        if (onMobile()) {
            getMobileDriver().hideKeyboard()
        }
    }

    fun clickContinueButton() {
        continueButton.element.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }

    fun clickSkipThrottlingLink() {
        alreadyUsingAppLink.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }

    fun isSearchCriteriaErrorMessageShown(): Boolean {
        return criteriaErrorMessage.element.isVisible
    }
}
