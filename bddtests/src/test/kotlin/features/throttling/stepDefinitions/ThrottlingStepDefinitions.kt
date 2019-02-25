package features.throttling.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.data.nhsAzureSearchData.NhsAzureSearchData.DEFAULT_LATITUDE
import mocking.data.nhsAzureSearchData.NhsAzureSearchData.DEFAULT_LONGITUDE
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.nhsAzureSearchService.FILTER_LOCAL_TYPE_POSTCODE
import mocking.nhsAzureSearchService.FILTER_TYPE_POSTCODE_OUT_CODE
import mocking.nhsAzureSearchService.NHSAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationRequestBody
import mocking.nhsAzureSearchService.NhsAzureSearchPostcodesAndPlacesRequestBody
import mocking.nhsAzureSearchService.SELECT_ORGANISATIONS_GEOCODE_SEARCH
import mocking.nhsAzureSearchService.getGeoDistanceFilterForLatLon
import mocking.nhsAzureSearchService.getGeoDistanceOrderbyForLatLon
import models.Patient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertTrue
import pages.assertIsVisible
import pages.loggedOut.CIDAccountCreationPage
import pages.throttling.GPFinderPage
import pages.throttling.GPParticipationPage
import pages.throttling.GPSearchResultsPage
import pages.loggedOut.LoginPage
import pages.throttling.SendingEmailPage
import pages.throttling.WaitingListJoinedPage

private const val TECHNICAL_PROBLEMS = "Technical problems"
private const val TOO_MANY_RESULTS = "Too many results"
private const val NO_RESULTS_FOUND = "No results found"
private const val NO_RESULTS_COUNT = 0
private const val MAX_ORGANISATION_RESULTS_COUNT = NhsAzureSearchData.ORGANISATION_LIMIT
private const val SOME_RESULTS_COUNT = 2
private const val POSTCODE_SEARCH_RESULTS_COUNT = 1
private const val FULL_POSTCODE_WITH_SPACE = "SW9 1NG"
private const val FULL_POSTCODE_WITHOUT_SPACE = "SW91NG"
private const val FULL_POSTCODE_MIXED_CASE_NO_SPACE = "Sw91ng"
private const val OUTWARD_CODE = "SW9"
private const val NO = "no"
private const val BLANK = "blank"
private const val MULTIPLE = "multiple"
private const val MAXIMUM_LIMIT = "the maximum limit"
private const val MORE_THAN_MAXIMUM = "more than the maximum"

open class ThrottlingStepDefinitions {

    private val mockingClient = MockingClient.instance
    private var searchText = ""

    @Steps
    lateinit var browser: BrowserSteps

    lateinit var cidAccountCreationPage: CIDAccountCreationPage
    lateinit var login: LoginPage
    lateinit var gpFinderPage: GPFinderPage
    lateinit var gpSearchResultsPage: GPSearchResultsPage
    lateinit var gpParticipationPage: GPParticipationPage
    lateinit var sendingEmailPage: SendingEmailPage
    lateinit var waitingListJoinedPage: WaitingListJoinedPage

    @Given("^I see the GP Search Results Page with " +
            "($NO_RESULTS_COUNT|$SOME_RESULTS_COUNT|$MAX_ORGANISATION_RESULTS_COUNT|$POSTCODE_SEARCH_RESULTS_COUNT) " +
            "search results$")
    fun iSeeTheGPSearchResultsPage(numResults: String) {
        assertTrue(gpSearchResultsPage.resultsExistForSearch(numResults.toInt()))
    }

    @Given("^There is a GP Practice with a postcode like " +
            "($FULL_POSTCODE_WITHOUT_SPACE|$FULL_POSTCODE_WITH_SPACE|$FULL_POSTCODE_MIXED_CASE_NO_SPACE" +
            "|$OUTWARD_CODE)$")
    fun thereIsAtLeastOneGPPracticeWithAPostcodeLike(postcode: String) {
        var expectedSearch = postcode
        var filterType = FILTER_LOCAL_TYPE_POSTCODE
        when (postcode) {
            OUTWARD_CODE -> {
                expectedSearch = OUTWARD_CODE
                filterType = FILTER_TYPE_POSTCODE_OUT_CODE
            }
        }
        searchText = postcode
        mockingClient.forNhsAzureSearchPostcodesAndPlaces {
            nhsAzureSearch.nhsAzureSearchPostcodesAndPlacesRequest(NhsAzureSearchPostcodesAndPlacesRequestBody(
                    search = "\"$expectedSearch\"",
                    filter = filterType
            )).respondWithSuccess(NhsAzureSearchData.getSuccessfulPostcodeMatch())
        }
        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    filter = getGeoDistanceFilterForLatLon(DEFAULT_LATITUDE, DEFAULT_LONGITUDE),
                    select = SELECT_ORGANISATIONS_GEOCODE_SEARCH,
                    search = null,
                    searchFields = null,
                    queryType = null,
                    orderby = getGeoDistanceOrderbyForLatLon(DEFAULT_LATITUDE, DEFAULT_LONGITUDE)
            )).respondWithSuccess(NhsAzureSearchData.getOrganisationWithinRange())
        }
    }

    @Given("^There are ($MULTIPLE|$NO|$MAXIMUM_LIMIT|$MORE_THAN_MAXIMUM) GP Practices for my search criteria$")
    fun thereAreXGPPracticesForMySearchCriteria(howManyPractices: String) {
        var data = NHSAzureSearchOrganisationReply()
        when(howManyPractices) {
            MULTIPLE -> {
                data = NhsAzureSearchData.getLessThanMaxNumberOfOrganisationData()
            }
            NO -> {
                data = NhsAzureSearchData.getZeroOrganisationData()
            }
            MAXIMUM_LIMIT -> {
                data = NhsAzureSearchData.getMaxNumberOfOrganisationData()
            }
            MORE_THAN_MAXIMUM -> {
                data = NhsAzureSearchData.getMoreThanMaxNumberOfOrganisationData()
            }
        }
        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    search = "${GPFinderPage.validSearch}*"))
                    .respondWithSuccess(data)
        }
        searchText = GPFinderPage.validSearch
    }

    @Given("^The NHS Service Search is unavailable$")
    fun iSearchForAGPPracticeWhenTheNHSServiceSearchIsUnavailable() {
        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    search = "${GPFinderPage.validSearch}*"))
                    .respondWithServiceUnavailable()
        }
        searchText = GPFinderPage.validSearch
    }

    @Given("^I submit my search")
    fun iSubmitMySearch() {
        gpFinderPage.enterSearchTerm(searchText)
        gpFinderPage.clickContinueButton()
    }

    @Given("^I submit ($NO|$BLANK) search criteria$")
    fun iSubmitInvalidSearchCriteria(noOrBlank: String) {
        when(noOrBlank) {
            NO -> {
                searchText = GPFinderPage.emptyInvalidSearch
            }
            BLANK -> {
                searchText = GPFinderPage.blankInvalidSearch
            }
        }
        iSubmitMySearch()
    }

    @Given("I am not logged in and I have not completed the beta throttling flow$")
    open fun iHaveNotLoggedInAndIHaveNotPreviouslySelectedMyGPPractice() {
        browser.goToApp()
        gpFinderPage.driver.manage().deleteAllCookies()
        browser.goToApp()
    }

    @Given("^I have searched for my GP Practice$")
    fun iHaveSearchedForMyGPPractice() {
        assertGPFinderPageVisible()
        thereAreXGPPracticesForMySearchCriteria(MULTIPLE)
        iSeeTheGPSearchResultsPage("$SOME_RESULTS_COUNT")
    }

    @When("^My GP Practice (is|is not) participating in beta$")
    fun myGPPracticeIsOrIsNotParticipatingInBeta(isOrIsNot: String) {
        val participating = isOrIsNot == "is"
        gpSearchResultsPage.setPracticeToSelect(participating)
        gpParticipationPage.setHeaderToLookFor(participating)
    }

    @When("^I select my GP Practice$")
    fun iSelectMyGPPractice() {
        gpSearchResultsPage.selectMyGpPractice()
    }

    @When("^I see the Practice (Participating|Not Participating) page$")
    fun iSeeThePracticeParticipatingOrNotParticipatingPage(participating: String) {
        gpParticipationPage.featuresUsedHeader.assertIsVisible()
        gpParticipationPage.currentlyAvailableHeader.assertIsVisible()

        when(participating) {
            "Participating" -> {
                gpParticipationPage.ctaParticipatingContinueButton.assertIsVisible()
                gpParticipationPage.assertParticipatingFeaturesVisible()
                gpParticipationPage.createAccountMessage.assertIsVisible()
                gpParticipationPage.limitingFeaturesWarning.assertIsVisible()
            }
            "Not Participating" -> {
                gpParticipationPage.ctaNotParticipatingContinueButton.assertIsVisible()
                gpParticipationPage.assertNotParticipatingFeaturesVisible()
                gpParticipationPage.comingSoonHeader.assertIsVisible()
            }
        }
    }

    @When("^I click the This is not my GP surgery button$")
    fun iClickTheThisIsNotMyGPSurgeryButton() {
        gpParticipationPage.clickNotMySurgeryButton()
    }

    @When("^I click the link to skip the throttling flow$")
    fun iClickTheLinkToSkipTheThrottlingFlow() {
        gpFinderPage.clickSkipThrottlingLink()
    }

    @When("^I click the go to home screen button$")
    fun iClickTheGoToHomeScreenButton() {
        waitingListJoinedPage.homeButton.click()
    }

    @When("^I click the Continue button on the Practice Participating page$")
    fun whenIClickTheContinueButtonOnThePracticeParticipatingPage() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.jackJackson)
        gpParticipationPage.ctaParticipatingContinueButton.click()
    }

    @Then("^The ($TOO_MANY_RESULTS|$TECHNICAL_PROBLEMS|$NO_RESULTS_FOUND) error message (is|is not) visible$")
    fun theErrorMessageIsOrIsNotVisible(errorType: String, isOrIsNot: String) {
        when (errorType) {
            TECHNICAL_PROBLEMS -> {
                gpSearchResultsPage.technicalProblemsErrorHeaderIsVisible(isOrIsNot == "is")
            }
            TOO_MANY_RESULTS -> {
                gpSearchResultsPage.tooManyResultsErrorHeaderIsVisible(isOrIsNot == "is")
            }
            NO_RESULTS_FOUND -> {
                gpSearchResultsPage.noResultsFoundErrorHeaderIsVisible(isOrIsNot == "is")
            }
        }
    }

    @Then("^I see the GP Finder page with a search criteria error message$")
    fun iSeeTheGPFinderPageWithASearchCriteriaErrorMessage() {
        assertTrue(gpFinderPage.isSearchCriteriaErrorMessageShown())
    }

    @Then("^I see the CID login page$")
    fun iSeeTheCIDLoginPage() {
        assertTrue(cidAccountCreationPage.isVisible())
    }

    @When("^I click the Practice Not Participating continue button$")
    fun iClickThePracticeNotParticipatingContinueButton(){
        gpParticipationPage.ctaNotParticipatingContinueButton.click()
    }

    @Then("^I see the Sending Email Page$")
    fun iSeeTheSendingEmailPage() {
        sendingEmailPage.waitingListResultsHeader.assertIsVisible()
        sendingEmailPage.emailFeatureText.assertIsVisible()
        sendingEmailPage.emailText.assertIsVisible()
        sendingEmailPage.continueButton.assertIsVisible()
    }

    @Then("^I click the back button on Sending Email page$")
    fun iClickTheBackButtonOSendingEmailPage() {
        sendingEmailPage.backLink.click()
    }

    @And("^The brothermailer service will return a successful response")
    fun theBrotherMailerServiceReturnsASuccessfulResponse()
    {
        mockingClient.forBrotherMailer {
            postToBrotherMailer().respondWithOkResponse()
        }
    }

    @When("^I enter (a valid|an invalid) email and submit$")
    fun iEnterAValidOrInvalidEmailAndSubmit(validOrInvalid: String){
        when (validOrInvalid) {
            "a valid" -> {
                sendingEmailPage.enterEmail(SendingEmailPage.validEmail)
            }
            "an invalid" -> {
                sendingEmailPage.enterEmail(SendingEmailPage.invalidEmail)
            }
        }
        iClickTheContinueButtonOnTheSendingEmailPage()
    }

    @When("^I click the back button on the Sending Email Results Page$")
    fun iClickTheBackButtonOnTheSendingEmailResultsPage(){
        waitingListJoinedPage.homeButton.click()
    }

    @When("^I choose (to|not to) sign up to brothermailer$")
    fun iChooseToOrNotToSignUpToBrotherMailer(toOrNotTo: String) {
        when (toOrNotTo) {
            "to" -> {
                sendingEmailPage.yesRadioButton.click()
            }
            "not to" -> {
                sendingEmailPage.noRadioButton.click()
            }
        }
    }

    @When("^I click the continue button on the Sending Email page$")
    fun iClickTheContinueButtonOnTheSendingEmailPage() {
        sendingEmailPage.continueButton.click()
    }

    @Then("^I see the invalid email error$")
    fun iSeeTheInvalidEmailError() {
        sendingEmailPage.isInvalidEmailVisible()
    }

    @Then("^I see the login page for practice not participating$")
    fun iCanSeeTheLoginPage() {
        login.throttlingNotParticipatingHeader.assertIsVisible()
    }

    @Then("^I see the Waiting List (Joined|Not Joined) page$")
    fun iSeeTheSendingEmailResultsPage(joinedOrNot: String) {
        waitingListJoinedPage.waitingListResultsHeader.assertIsVisible()
        waitingListJoinedPage.whatNextTitle.assertIsVisible()
        waitingListJoinedPage.whatToDoTitle.assertIsVisible()
        when (joinedOrNot) {
            "Joined" -> {
                waitingListJoinedPage.whatNextJoinedParagraph.assertIsVisible()
            }
            "Not Joined" -> {
                waitingListJoinedPage.whatNextNotJoinedParagraph.assertIsVisible()
            }
        }
        waitingListJoinedPage.homeButton.assertIsVisible()
    }

    @And("^The brothermailer service is down$")
    fun theBrotherMailerServiceIsDown() {
        mockingClient.forBrotherMailer {
            postToBrotherMailer().respondWithNotFoundError()
        }
    }

    @Then("^I see the brothermailer service is down error$")
    fun iSeeTheBrotherMailerServiceIsDown() {
        val message = sendingEmailPage.inLineError.element.text
        Assert.assertEquals("There was a problem adding you. Please try again", message)
    }

    @Then("^I see the GP Finder Page$")
    fun assertGPFinderPageVisible() {
        assertTrue(gpFinderPage.isFindYourGPSurgeryHeaderVisible())
    }

    @Then("^The GP Practice found matches the searched postcode$")
    fun theGPPracticeFoundMatchesThePostcode() {
        assertTrue(gpSearchResultsPage.gpPracticeFoundByPostcodeIsVisible())
    }

    @Then("^I see the make a choice error$")
    fun iSeeTheMakeAChoiceError() {
        sendingEmailPage.choiceError.assertIsVisible()
    }
}

