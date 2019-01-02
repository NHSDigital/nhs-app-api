package pages.throttling

import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import pages.HybridPageObject
import pages.HybridPageElement

class GPSearchResultsPage : HybridPageObject() {

    companion object {
        const val TECHNICAL_PROBLEMS_ERROR_HEADER = "We are experiencing technical problems"
        const val TOO_MANY_RESULTS_TEXT = "Can't find your GP surgery?"
        const val NO_RESULTS_FOUND_TEXT = "No results found"
        const val FULL_POSTCODE_WITH_SPACE = "SW9 1NG"
    }

    private var gpPracticeToSelect: HybridPageElement = HybridPageElement(
        browserLocator = "",
        androidLocator = null,
        page = this
    )

    private val technicalProblemsErrorHeader = HybridPageElement(
        browserLocator = "//h3[contains(text(), \"$TECHNICAL_PROBLEMS_ERROR_HEADER\")]",
        androidLocator = null,
        page = this
    )

    private val noResultsFoundErrorHeader = HybridPageElement(
        browserLocator = "//h2[contains(text(), \"$NO_RESULTS_FOUND_TEXT\")]",
        androidLocator = null,
        page = this
    )

    private val tooManyResultsErrorHeader = HybridPageElement(
        browserLocator = "//h2[contains(text(), \"$TOO_MANY_RESULTS_TEXT\")]",
        androidLocator = null,
        page = this
    )

    private val searchResults = HybridPageElement(
        browserLocator = "//ul[@id='searchResults']/li",
        androidLocator = null,
        page = this
    )

    private val participatingGPPractice = HybridPageElement(
        browserLocator = "//ul[@id='searchResults']/li/a[@id='btnGpPractice-F81090']",
        androidLocator = null,
        page = this
    )

    private val notParticipatingGPPractice = HybridPageElement(
        browserLocator = "//ul[@id='searchResults']/li/a[@id='btnGpPractice-F81091']",
        androidLocator = null,
        page = this
    )

    private val foundGPPracticeByPostcode = HybridPageElement(
        browserLocator = "//ul[@id='searchResults']//p[contains(text(),'$FULL_POSTCODE_WITH_SPACE')]",
        androidLocator = null,
        page = this
    )

    fun technicalProblemsErrorHeaderIsVisible(isVisible: Boolean) {
        if (isVisible) {
            assertTrue(technicalProblemsErrorHeader.element.isVisible)
        } else {
            assertFalse(findByXpath(technicalProblemsErrorHeader.browserLocator).isVisible)
        }
    }

    fun noResultsFoundErrorHeaderIsVisible(isVisible: Boolean) {
        if (isVisible) {
            assertTrue(noResultsFoundErrorHeader.element.isVisible)
        } else {
            assertFalse(findByXpath(noResultsFoundErrorHeader.browserLocator).isVisible)
        }
    }

    fun tooManyResultsErrorHeaderIsVisible(isVisible: Boolean) {
        if (isVisible) {
            assertTrue(tooManyResultsErrorHeader.element.isVisible)
        } else {
            assertFalse(findByXpath(tooManyResultsErrorHeader.browserLocator).isVisible)
        }
    }

    fun selectMyGpPractice() {
        gpPracticeToSelect.click()
    }

    fun resultsExistForSearch(count: Int): Boolean {
        return searchResults.elements.size == count
    }

    fun setPracticeToSelect(participating: Boolean) {
        gpPracticeToSelect = if (participating) participatingGPPractice else notParticipatingGPPractice
    }

    fun gpPracticeFoundByPostcodeIsVisible(): Boolean {
        return foundGPPracticeByPostcode.element.isVisible
    }

}