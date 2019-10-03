package features.authentication.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.thucydides.core.annotations.Steps
import org.joda.time.DateTime
import pages.navigation.WebHeader
import utils.SerenityHelpers


class AuthenticationErrorStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var webHeader: WebHeader

    val mockingClient = MockingClient.instance


    @Given("^I am logged in as a (.*) user$")
    fun iAmLoggedInTo(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I am logged in as a (.*) user created before Im1 Cache Keys existed$")
    fun iAmLoggedInWithoutIm1CacheKey(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(im1ConnectionToken = null)
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I attempt to log in as a (.*) user without an NHS Number$")
    fun iAmLoggedInToWithoutNHSNumber(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(nhsNumbers = emptyList())
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I attempt to log in as a (.*) user without a date of birth$")
    fun iAmLoggedInToWithoutDOB(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(dateOfBirth = "")
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I attempt to log in as a (.*) user with an age under (\\d+)$")
    fun iAmLoggedInToWithAgeUnderMinAge(gpSystem: String, age: Int) {
        val birthdayToday = DateTime.now().minusYears(age);
        val birthdayTomorrow = birthdayToday.plusDays(1)
        val dateOfBirth = birthdayTomorrow.toString(DateTimeFormats.dateWithoutTimeFormat)
        val patient = Patient.getDefault(gpSystem).copy(dateOfBirth = dateOfBirth)
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I attempt to log in as a (.*) user that is (\\d+)$")
    fun iAmLoggedInToWithUserOfAge(gpSystem: String, age: Int) {
        val dateOfBirth = DateTime.now().minusYears(age)
                .toString(DateTimeFormats.dateWithoutTimeFormat)
        val patient = Patient.getDefault(gpSystem).copy(dateOfBirth = dateOfBirth)
        setupAndLogIn(patient, gpSystem)
    }

    @Given("I attempt to log in as a (.*) user with invalid ODS Code$")
    fun iAttemptToLogInWithInvalidOdsCode(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem).copy(odsCode = "A33224")
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I attempt to log in as an EMIS and the CID request timeout$")
    fun loginAsEmisTimeout() {
        val gpSystem = "EMIS"
        val patient = Patient.getDefault(gpSystem).copy(odsCode = "A33224")
        setupAndTimeout(patient, gpSystem)
    }

    @Given("I attempt to log in as an EMIS user with no userPatientLinkToken$")
    fun iAttemptToLogInAsEmisWithNoUserPatientLinkToken() {
        val patient = Patient.getDefault("EMIS").copy(userPatientLinkToken = "")
        setupAndLogIn(patient, "EMIS")
    }

    private fun setupAndLogIn(patient: Patient, gpSystem: String) {
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        browser.goToApp()
        login.using(patient)
    }

    private fun setupAndTimeout(patient: Patient, gpSystem: String) {
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createTimeoutfor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        browser.goToApp()
        login.using(patient)
    }

}
