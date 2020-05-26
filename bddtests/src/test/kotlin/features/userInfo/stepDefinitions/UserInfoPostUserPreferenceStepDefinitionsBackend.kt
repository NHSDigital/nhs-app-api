package features.userInfo.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.patients.PatientHandler
import utils.SerenityHelpers
import utils.getOrFail
import utils.set

class UserInfoPostUserPreferenceStepDefinitionsBackend {

    private val mockingClient = MockingClient.instance

    @Given("^I am an api user wishing to post my user research preference 'OptIn'$")
    fun iAmAnApiUserWishingToPostMyUserResearchPreferenceOptIn() {
        setUpPatient()
        UserInfoSerenityHelpers.EXPECTED_USER_RESEARCH_PREFERENCE.set("OptIn")
        mockingClient.forQualtrics.mock { respondWithSuccess() }
    }

    @Given("^I am an api user wishing to post my user research preference 'OptOut'$")
    fun iAmAnApiUserWishingToPostMyUserResearchPreferenceOptOut() {
        setUpPatient()
        UserInfoSerenityHelpers.EXPECTED_USER_RESEARCH_PREFERENCE.set("OptOut")
    }

    @Given("^I am an api user wishing to post my user research preference 'OptIn' but qualtrics will return an error$")
    fun iAmAnApiUserWishingToPostMyUserResearchPreferenceButQualtricsWillReturnAnError() {
        setUpPatient()
        UserInfoSerenityHelpers.EXPECTED_USER_RESEARCH_PREFERENCE.set("OptIn")
        mockingClient.forQualtrics.mock { respondWithServerError() }
    }

    @Given("^I am an api user wishing to post my user research preference 'OptIn' but without an email")
    fun iAmAnApiUserWishingToPostMyUserResearchPreferenceButWithoutAnEmail() {
        setUpPatient(withEmail = false)
        UserInfoSerenityHelpers.EXPECTED_USER_RESEARCH_PREFERENCE.set("OptIn")
    }

    @When("^I post my user research preference to the user info endpoint$")
    fun iPostMyUserResearchPreferenceToTheUserInfoEndpoint() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val preference = UserInfoSerenityHelpers.EXPECTED_USER_RESEARCH_PREFERENCE.getOrFail<String>()
        UserInfoApi.postUserResearch(authToken, preference)
    }

    @When("^I post my user research preference to the user info endpoint without an access token$")
    fun iPostTheUserInterruptSettingsInfoEndpointWithoutAuthToken() {
        val preference = UserInfoSerenityHelpers.EXPECTED_USER_RESEARCH_PREFERENCE.getOrFail<String>()
        UserInfoApi.postUserResearch(null, preference)
    }

    @Then("^posting user research preferences with an invalid access token will return an " +
            "Unauthorised error$")
    fun iPostTheInterruptSettingsFromUserInfoEndpointWithInvalidAuthToken() {
        val preference = UserInfoSerenityHelpers.EXPECTED_USER_RESEARCH_PREFERENCE.getOrFail<String>()
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { accessToken ->
            UserInfoApi.postUserResearch(authToken = accessToken, preference = preference)
        }
    }

    private fun setUpPatient(withEmail: Boolean = true) {
        val gpSystem = Supplier.EMIS
        val patientToUse = PatientHandler.getForSupplier(gpSystem).getDefault()
        patientToUse.contactDetails.emailAddress = if (withEmail) "Email" else ""
        SerenityHelpers.setGpSupplier(gpSystem)
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney().createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(gpSystem).createFor(patientToUse)
    }
}
