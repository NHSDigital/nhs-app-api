package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.authentication.steps.AuthReturnSteps
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.AuthReturnPage

class AuthReturnStepDefinitions : AbstractSteps() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps

    private lateinit var patient: Patient

    @Steps
    lateinit var authReturn: AuthReturnSteps

    lateinit var authReturnPage: AuthReturnPage

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
    fun thenISeeTheAppropriateErrorMessageForACourseRequestError() {

        val pageTitle = ""
        val pageHeader = ""
        val header = authReturnPage.errorH1
        val subHeader = authReturnPage.errorH2
        val message = authReturnPage.errorParagraph
        val retryButtonText = authReturnPage.errorCtaText

        authReturn.assertCorrectErrorMessageShown(pageTitle, pageHeader, header, subHeader, message, retryButtonText)
    }

    @Then("I click on the navigation button")
    fun thenICLickOnTheNavigationButton() {
        val retryButtonText = authReturnPage.errorCtaText
        authReturnPage.clickOnButtonContainingText(retryButtonText)
    }

}
