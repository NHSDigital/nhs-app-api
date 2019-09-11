package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.thucydides.core.annotations.Steps
import utils.SerenityHelpers

class AuthenticationWithLinkedProfilesStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps

    val mockingClient = MockingClient.instance

    @Given("^I am logged in as a (.*) user with linked profiles$")
    fun iAmLoggedInWithLinkedProfiles(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        setupWithLinkedAccountsAndLogIn(patient, gpSystem)
    }

    private fun setupWithLinkedAccountsAndLogIn(patient: Patient, gpSystem: String) {
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createForWithLinkedAccounts(patient)

        browser.goToApp()
        login.using(patient)
    }

    @Then("^I see the linked profiles link$")
    fun iSeeLinkedProfilesLink() {
        home.assertLinkedProfileLinkVisible()
    }

}
