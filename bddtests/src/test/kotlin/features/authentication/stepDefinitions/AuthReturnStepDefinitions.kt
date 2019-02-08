package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.loggedOut.AuthReturnPage
import pages.ErrorPage

class AuthReturnStepDefinitions : AbstractSteps() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps

    private lateinit var patient: Patient

    lateinit var authReturnPage: AuthReturnPage
    lateinit var errorPage: ErrorPage

    @Given("^I am logged into Citizen ID but am receiving invalid data$")
    fun loggedInInCitizenIdInvalidData() {
        this.patient = Patient.paulSmith

        CitizenIdSessionCreateJourney(mockingClient).createInvalidFor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am logged into Citizen ID but GP System authentication fails$")
    fun loggedInInCitizenIdGpAuthenticationFails() {
        this.patient = Patient.kevinBarry

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am logged into Citizen ID but GP System session cannot be established$")
    fun loggedInInCitizenIdSessionNotEstablished() {
        this.patient = Patient.montelFrye

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)

        browser.goToApp()
        login.using(this.patient)
    }

    @Then("I see the appropriate error message for a login error")
    fun thenISeeTheAppropriateErrorMessageForALoginError() {

        val header = authReturnPage.errorH1
        val subHeader = authReturnPage.errorH2
        val message = authReturnPage.errorParagraph1
        val errorDetail = authReturnPage.errorParagraph2
        val retryButtonText = authReturnPage.errorCtaText

        errorPage.assertHeaderText(header)
                .assertSubHeaderText(subHeader)
                .assertMessageText(message)
                .assertRetryButtonText(retryButtonText)
                .assertErrorDetailText(errorDetail)

    }

    @Then("I click on the navigation button")
    fun thenICLickOnTheNavigationButton() {
        val retryButtonText = authReturnPage.errorCtaText
        authReturnPage.clickOnButtonContainingText(retryButtonText)
    }

}
