package features.throttling.stepDefinitions

import cucumber.api.java.en.Given
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchRequestBody
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.throttling.GPFinderPage
import pages.throttling.GPSearchResultsPage

open class ThrottlingStepDefinitions {

    val mockingClient = MockingClient.instance

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var gpFinderPage: GPFinderPage

    lateinit var gpSearchResultsPage: GPSearchResultsPage

    @Given("^the NHS Azure Search Service has GP Practices$")
    fun givenTheNHSAzureSearchServiceHasGpPractices() {

        mockingClient.forNhsAzureSearch {
            nhsAzureSearch.nhsAzureSearchRequest(NhsAzureSearchRequestBody(
                    search = "Chesterfield"))
                    .respondWithSuccess(NhsAzureSearchData.getNhsAzureSearchData())
        }
    }

    @Given("^I see the GP Finder Page$")
    fun assertGPFinderPageVisible() {
        Assert.assertTrue(gpFinderPage.isFindYourGPSurgeryHeaderVisible())
    }

    @Given("^I see the GP Search Results Page with a search results$")
    fun assertGPSurgeryResultsHeaderVisible() {
        Assert.assertTrue(gpSearchResultsPage.isGPResultsHeaderVisible())
        Assert.assertTrue(gpSearchResultsPage.testResultsExistForSearch(2))
    }

    @Given("^I search for a GP Practice$")
    fun iSearchForAGPPractice() {
        gpFinderPage.enterSearchTerm("Chesterfield")
        gpFinderPage.clickContinueButton()
    }

    @Given("^I am not logged in and I have not selected previously my GP Practice$")
    open fun iHaveNotLoggedIn() {
        browser.goToApp()
    }
}

