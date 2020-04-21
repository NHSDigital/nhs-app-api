package features.termsAndConditions.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.termsAndConditions.TermsAndConditionsJourneyFactory
import models.Patient
import org.joda.time.DateTime
import utils.SerenityHelpers
import pages.TermsAndConditionsPage
import pages.UpdatedTermsAndConditionsPage
import pages.assertIsVisible
import java.util.*

class TermsAndConditionsStepDefinitions {

    private lateinit var termsAndConditionsPage: TermsAndConditionsPage
    private lateinit var updatedTermsAndConditionsPage: UpdatedTermsAndConditionsPage

    val mockingClient = MockingClient.instance

    @Given("^I am an? (.*) patient who has not already accepted terms and conditions$")
    fun iHaveNotAcceptedTermsAndConditions(gpSystem: String) {
        initialisePatientAndGpSystem(gpSystem)
        TermsAndConditionsJourneyFactory.noConsent()
    }

    @Given("^I am an? (.*) patient who has already accepted terms and conditions$")
    fun iHaveAlreadyAcceptedTermsAndConditions(gpSystem: String) {
        val patient = initialisePatientAndGpSystem(gpSystem)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @Given("^I am an? (.*) patient who has accepted terms and conditions but updated terms and conditions exist$")
    fun iHavePreviouslyAcceptedTermsAndConditionsAndUpdatedAcceptanceIsRequired(gpSystem: String) {
        val patient = initialisePatientAndGpSystem(gpSystem)
        TermsAndConditionsJourneyFactory.consent(patient, DateTime.parse("2018-11-11T00:00:00+00:00"))
    }

    @When("^I click the continue button on Terms and Conditions$")
    fun iClickTheContinueButtonOnTermsAndConditions() {
        termsAndConditionsPage.continueButton.click()
    }

    @When("^I click the continue button on Updated Terms and Conditions$")
    fun iClickTheContinueButtonOnUpdateTermsAndConditions() {
        updatedTermsAndConditionsPage.assertPageHeaderIsVisible()
        updatedTermsAndConditionsPage.continueButton.click()
    }

    @When("^I check the agree to terms and conditions checkbox$")
    fun iAgreeToTheTermsAndConditions() {
        termsAndConditionsPage.acceptTermsAndConditionsCheckBox.click()
    }

    @When("^I agree to the updated terms and conditions$")
    fun iAgreeToTheUpdatedTermsAndConditions() {
        updatedTermsAndConditionsPage.assertPageHeaderIsVisible()
        updatedTermsAndConditionsPage.acceptTermsAndConditionsCheckBox.click()
        updatedTermsAndConditionsPage.continueButton.click()
    }

    @Then("^the Terms and Conditions page is displayed$")
    fun theTermsAndConditionsPageIsDisplayed() {
        termsAndConditionsPage.assertPageHeaderIsVisible()
        termsAndConditionsPage.mainBodyText.assertIsVisible()
        termsAndConditionsPage.acceptTermsAndConditionsCheckBox.assertIsVisible()
        termsAndConditionsPage.continueButton.assertIsVisible()
    }

    @Then("^the updated Terms and Conditions page is displayed$")
    fun theUpdatedTermsAndConditionsPageIsDisplayed() {
        updatedTermsAndConditionsPage.assertPageHeaderIsVisible()
        updatedTermsAndConditionsPage.mainBodyText.assertIsVisible()
        updatedTermsAndConditionsPage.acceptTermsAndConditionsCheckBox.assertIsVisible()
        updatedTermsAndConditionsPage.continueButton.assertIsVisible()
    }

    @Then("^I see error messages indicating I have not yet accepted the terms and conditions$")
    fun iSeeErrorMessages() {
        termsAndConditionsPage.checkboxErrorMessage.assertIsVisible()
        termsAndConditionsPage.mainErrorMessage.assertIsVisible()
    }

    @Then("^I see error messages indicating I have not yet accepted the updated terms and conditions$")
    fun iSeeUpdatedErrorMessages() {
        updatedTermsAndConditionsPage.assertPageHeaderIsVisible()
        updatedTermsAndConditionsPage.checkboxErrorMessage.assertIsVisible()
        updatedTermsAndConditionsPage.mainErrorMessage.assertIsVisible()
    }

    private fun initialisePatientAndGpSystem(gpSystem: String): Patient {
        val supplier = Supplier.valueOf(gpSystem)

        // The integration tests use a single 'real' Cosmos collection so a unique id must be used to avoid
        // test runs affecting each other.
        val patient = Patient.getDefault(supplier).copy(nhsNumbers = listOf(UUID.randomUUID().toString()))

        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)

        return patient
    }
}
