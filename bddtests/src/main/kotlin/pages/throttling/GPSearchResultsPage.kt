package pages.throttling

import pages.HybridPageObject
import pages.HybridPageElement

open class GPSearchResultsPage : HybridPageObject() {

    companion object {
        val myGpOdsCode = "C81056"
    }

    val gpSearchResultsHeader = HybridPageElement(
            browserLocator = "//h1[contains(text(),'Select your GP surgery')]",
            androidLocator = null,
            page = this
    )

    val searchResults =
            HybridPageElement(
                    browserLocator = "//ul[@id='searchResults']/li",
                    androidLocator = null,
                    page = this)

    val gpPractice =
            HybridPageElement(
                    browserLocator = "//ul[@id='searchResults']/li/a[@id='btnGpPractice-$myGpOdsCode']",
                    androidLocator = null,
                    page = this)

    fun isGPResultsHeaderVisible(): Boolean {
        return gpSearchResultsHeader.element.isDisplayed
    }

    fun testResultsExistForSearch(count: Int): Boolean {
        return searchResults.elements.count() == count
    }


}