package pages.throttling

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.BannerObject
import pages.sharedElements.TextBlockElement

class GPSearchResultsPage : HybridPageObject() {

    companion object {
        const val FULL_POSTCODE_WITH_SPACE = "SW9 1NG"
    }

    private val errorBanner = BannerObject.error(this)

    private val searchResults = HybridPageElement(
            webDesktopLocator = "//ul[@id='searchResults']/li",
            webMobileLocator = "//ul[@id='searchResults']/li",
            androidLocator = null,
            page = this
    )

    val participatingGPPractice = HybridPageElement(
            webDesktopLocator = "//ul[@id='searchResults']/li/a[@id='btnGpPractice-F81090']",
            webMobileLocator = "//ul[@id='searchResults']/li/a[@id='btnGpPractice-F81090']",
            androidLocator = null,
            page = this
    )

    val notParticipatingGPPractice = HybridPageElement(
            webDesktopLocator = "//ul[@id='searchResults']/li/a[@id='btnGpPractice-F81091']",
            webMobileLocator = "//ul[@id='searchResults']/li/a[@id='btnGpPractice-F81091']",
            androidLocator = null,
            page = this
    )

    val foundGPPracticeByPostcode = HybridPageElement(
            webDesktopLocator = "//ul[@id='searchResults']//p[contains(text(),'$FULL_POSTCODE_WITH_SPACE')]",
            webMobileLocator = "//ul[@id='searchResults']//p[contains(text(),'$FULL_POSTCODE_WITH_SPACE')]",
            androidLocator = null,
            page = this
    )

    fun noResultsFoundErrorHeaderIsVisible() {
        TextBlockElement.withH2Header("No results found", this)
                .assert("We found no GP surgeries near \"Chesterfield\".")
    }

    fun tooManyResultsErrorHeaderIsVisible(isVisible: Boolean) {
        val tooManyResultsMessage = TextBlockElement.withH2Header("Can't find your GP surgery?",this)
        if (isVisible) {
            tooManyResultsMessage.assert("We can only show 20 results for what you search. " +
                    "The more specific your search, the better the results.")
        } else {
            tooManyResultsMessage.assertElementNotPresent()
        }
    }

    fun assertNumberOfResults(expectedResults: Int) {
        val actualResults = searchResults.waitForElement().elements
        Assert.assertEquals("Number of search results", expectedResults, actualResults.size)
    }

    fun assertNoResults() {
        val actualResults = searchResults.elements
        Assert.assertEquals("Number of search results", 0, actualResults.size)
    }

    fun assertTechnicalProblemsBanner() {
        errorBanner.assertVisible(arrayListOf("We are experiencing technical problems",
                "Something has gone wrong with this service. It wasn't your fault.",
                "Come back later. If it still isn't working then, contact us about the problem."))
    }
}
