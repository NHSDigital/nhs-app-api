package features.throttling.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.data.nhsAzureSearchData.NhsAzureSearchData.DEFAULT_LATITUDE
import mocking.data.nhsAzureSearchData.NhsAzureSearchData.DEFAULT_LONGITUDE
import mocking.nhsAzureSearchService.FILTER_LOCAL_TYPE_POSTCODE
import mocking.nhsAzureSearchService.FILTER_TYPE_POSTCODE_OUT_CODE
import mocking.nhsAzureSearchService.NHSAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationRequestBody
import mocking.nhsAzureSearchService.NhsAzureSearchPostcodesAndPlacesRequestBody
import mocking.nhsAzureSearchService.SELECT_ORGANISATIONS_GEOCODE_SEARCH
import mocking.nhsAzureSearchService.getGeoDistanceFilterForLatLon
import mocking.nhsAzureSearchService.getGeoDistanceOrderbyForLatLon
import org.junit.Assert.assertTrue
import pages.assertIsVisible
import pages.loggedOut.CIDAccountCreationPage
import pages.loggedOut.LoginPage
import pages.throttling.GPFinderPage
import utils.set

private const val FULL_POSTCODE_WITH_SPACE = "SW9 1NG"
private const val FULL_POSTCODE_WITHOUT_SPACE = "SW91NG"
private const val FULL_POSTCODE_MIXED_CASE_NO_SPACE = "Sw91ng"
private const val OUTWARD_CODE = "SW9"
private const val NO = "no"
private const val MULTIPLE = "multiple"
private const val MAXIMUM_LIMIT = "the maximum limit"
private const val MORE_THAN_MAXIMUM = "more than the maximum"

open class ThrottlingStepDefinitions {

    private val mockingClient = MockingClient.instance

    lateinit var cidAccountCreationPage: CIDAccountCreationPage
    lateinit var login: LoginPage

    @Given("^The brothermailer service is down$")
    fun theBrotherMailerServiceIsDown() {
        mockingClient.forBrotherMailer {
            postToBrotherMailer().respondWithNotFoundError()
        }
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
        ThrottlingSerenityHelpers.SEARCH_TEXT.set(postcode)
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
        ThrottlingSerenityHelpers.SEARCH_TEXT.set(GPFinderPage.validSearch)
    }

    @Given("^The NHS Service Search is unavailable$")
    fun iSearchForAGPPracticeWhenTheNHSServiceSearchIsUnavailable() {
        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    search = "${GPFinderPage.validSearch}*"))
                    .respondWithServiceUnavailable()
        }
        ThrottlingSerenityHelpers.SEARCH_TEXT.set(GPFinderPage.validSearch)
    }

    @Given("^The brothermailer service will return a successful response")
    fun theBrotherMailerServiceReturnsASuccessfulResponse()
    {
        mockingClient.forBrotherMailer {
            postToBrotherMailer().respondWithOkResponse()
        }
    }

    @Then("^I see the login page for practice not participating$")
    fun iCanSeeTheLoginPage() {
        login.throttlingNotParticipatingHeader.assertIsVisible()
        login.recordMyOrganDonationDecisionLink.assertIsVisible()
    }

    @Then("^I see the CID login page$")
    fun iSeeTheCIDLoginPage() {
        assertTrue(cidAccountCreationPage.isVisible())
    }
}

