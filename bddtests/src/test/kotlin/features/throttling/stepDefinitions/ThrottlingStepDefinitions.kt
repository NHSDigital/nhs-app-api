package features.throttling.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchRequestBody
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.throttling.GPFinderPage
import pages.throttling.GPParticipationPage
import pages.throttling.GPSearchResultsPage
import redis.clients.jedis.Jedis

private const val REDIS_PORT = 6379

@Suppress("IMPLICIT_CAST_TO_ANY")
open class ThrottlingStepDefinitions {

    val mockingClient = MockingClient.instance

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var gpFinderPage: GPFinderPage
    lateinit var gpSearchResultsPage: GPSearchResultsPage
    lateinit var gpParticipationPage: GPParticipationPage

    @Given("^I see the GP Finder Page$")
    fun assertGPFinderPageVisible() {
        Assert.assertTrue(gpFinderPage.isFindYourGPSurgeryHeaderVisible())
    }

    @Given("^I see the GP Search Results Page with search results$")
    fun assertGPSurgeryResultsHeaderVisible() {
        Assert.assertTrue(gpSearchResultsPage.isGPResultsHeaderVisible())
        Assert.assertTrue(gpSearchResultsPage.testResultsExistForSearch(2))
    }

    @Given("^I search for a GP Practice$")
    fun iSearchForAGPPractice() {
        gpFinderPage.enterSearchTerm(GPFinderPage.validSearch)
        mockingClient.forNhsAzureSearch {
            nhsAzureSearch.nhsAzureSearchRequest(NhsAzureSearchRequestBody(
                    search = "\"${GPFinderPage.validSearch}\"*"))
                    .respondWithSuccess(NhsAzureSearchData.getNhsAzureSearchData())
        }
        gpFinderPage.clickContinueButton()
    }

    @Given("I am not logged in and I have not completed the beta throttling flow$")
    open fun iHaveNotLoggedInAndIHaveNotPreviouslySelectedMyGPPractice() {
        gpFinderPage.driver.manage().deleteCookieNamed("BetaCookie")
        browser.goToApp()
    }

    @Given("^I have searched for my GP Practice$")
    fun iHaveSearchedForMyGPPractice() {
        assertGPFinderPageVisible()
        iSearchForAGPPractice()
        assertGPSurgeryResultsHeaderVisible()
    }

    @When("^My GP Practice (is|is not) participating in beta$")
    fun myGPPracticeIsOrIsNotParticipatingInBeta(isOrIsNot: String) {
        Jedis("localhost", REDIS_PORT).use { jedis ->
            when (isOrIsNot) {
                "is" -> jedis.set(GPSearchResultsPage.myGpOdsCode, "emis")
                else -> jedis.del(GPSearchResultsPage.myGpOdsCode)
            }
        }
    }

    @When("^I select my GP Practice$")
    fun iSelectMyGPPractice() {
        gpSearchResultsPage.gpPractice.click()
    }

    @When("^I see the Practice (Participating|Not Participating) page$")
    fun iSeeThePracticeParticipatingOrNotParticipatingPage(participating: String) {
        gpParticipationPage.featuresUsedHeader.assertIsVisible()
        gpParticipationPage.practiceNameHeader.assertIsVisible()
        gpParticipationPage.practiceAddressParagraph.assertIsVisible()
        gpParticipationPage.currentlyAvailableHeader.assertIsVisible()

        when(participating) {
            "Participating" -> {
                gpParticipationPage.assertParticipatingFeaturesVisible()
                gpParticipationPage.ctaCreateAccountButton.assertIsVisible()
                gpParticipationPage.createAccountMessage.assertIsVisible()
                gpParticipationPage.limitingFeaturesWarning.assertIsVisible()
            }
            "Not Participating" -> {
                gpParticipationPage.comingSoonHeader.assertIsVisible()
                gpParticipationPage.assertNotParticipatingFeaturesVisible()
                gpParticipationPage.ctaContinueButton.assertIsVisible()
            }
        }
    }

    @When("^I click the This is not my GP surgery button$")
    fun iClickTheThisIsNotMyGpSurgeryButton() {
        gpParticipationPage.ctaNotMySurgeryButton.click()
    }

}

