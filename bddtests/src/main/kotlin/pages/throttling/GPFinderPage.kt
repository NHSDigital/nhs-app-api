package pages.throttling

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageObject
import pages.HybridPageElement
import pages.myrecord.SHRUB_ANIMATION_DURATION_MILLIS

open class GPFinderPage : HybridPageObject() {

    companion object {
        val validSearch = "Chesterfield"
    }

    val findYourGPSurgeryHeader = HybridPageElement(
            browserLocator = "//h4[contains(text(),'Find your GP surgery')]",
            androidLocator = null,
            page = this
    )

    private val searchTermField = HybridPageElement(
            browserLocator = "//*[@id='searchTextInput']",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Continue')]",
            androidLocator = null,
            page = this
    )

    fun isFindYourGPSurgeryHeaderVisible(): Boolean {
        return findYourGPSurgeryHeader.element.isDisplayed
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
}
