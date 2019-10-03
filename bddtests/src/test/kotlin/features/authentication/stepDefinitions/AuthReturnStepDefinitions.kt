package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import models.patients.EmisPatients
import models.Patient
import models.patients.TppPatients
import net.thucydides.core.annotations.Steps
import pages.ErrorPage
import pages.HybridPageObject
import pages.clickOnActionContainingText

class AuthReturnStepDefinitions : AbstractSteps() {
    val backToHomeText = "Back to home"
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps

    private lateinit var patient: Patient

    lateinit var pageActions: HybridPageObject
    lateinit var errorPage: ErrorPage

    @Given("^I am logged into Citizen ID but am receiving invalid data$")
    fun loggedInInCitizenIdInvalidData() {
        this.patient = EmisPatients.paulSmith

        CitizenIdSessionCreateJourney(mockingClient).createInvalidFor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am logged into Citizen ID but GP System authentication fails$")
    fun loggedInInCitizenIdGpAuthenticationFails() {
        this.patient = TppPatients.kevinBarry

        CitizenIdSessionCreateJourney(mockingClient).createInvalidAuthenticationTokenfor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am logged into Citizen ID but EMIS session cannot be established$")
    fun loggedInInCitizenIdSessionNotEstablished() {
        this.patient = EmisPatients.montelFrye

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Then("I click on the navigation button")
    fun thenICLickOnTheNavigationButton() {
        val retryButtonText = backToHomeText
        pageActions.clickOnButtonContainingText(retryButtonText)
    }

    @Then("I click on the navigation action")
    fun thenICLickOnTheNavigationAction() {
        val retryText = backToHomeText
        pageActions.clickOnActionContainingText(retryText)
    }
}
