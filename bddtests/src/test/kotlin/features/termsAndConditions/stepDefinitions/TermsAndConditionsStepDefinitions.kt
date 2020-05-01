package features.termsAndConditions.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.termsAndConditions.TermsAndConditionsJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import org.joda.time.DateTime
import utils.SerenityHelpers
import pages.TermsAndConditionsPage
import pages.UpdatedTermsAndConditionsPage
import pages.assertIsVisible

class TermsAndConditionsStepDefinitions {

    private lateinit var termsAndConditionsPage: TermsAndConditionsPage
    private lateinit var updatedTermsAndConditionsPage: UpdatedTermsAndConditionsPage

    @Given("^I am an? (.*) patient who has not already accepted terms and conditions$")
    fun iHaveNotAcceptedTermsAndConditions(gpSystem: String) {
        initialisePatientAndGpSystem(Supplier.valueOf(gpSystem))
        TermsAndConditionsJourneyFactory.noConsent()
    }

    @Given("^I am a patient with proof level 5 who has not already accepted terms and conditions$")
    fun iAmAPatientWithProofLevel5WhoHasNotAlreadyAcceptedTermsAndConditions() {
        initialisePatientAndGpSystem(Supplier.EMIS, IdentityProofingLevel.P5)
        TermsAndConditionsJourneyFactory.noConsent()
    }

    @Given("^I am an? (.*) patient who has already accepted terms and conditions$")
    fun iHaveAlreadyAcceptedTermsAndConditions(gpSystem: String) {
        val patient = initialisePatientAndGpSystem(Supplier.valueOf(gpSystem))
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @Given("^I am an? (.*) patient who has accepted terms and conditions but updated terms and conditions exist$")
    fun iHavePreviouslyAcceptedTermsAndConditionsAndUpdatedAcceptanceIsRequired(gpSystem: String) {
        val patient = initialisePatientAndGpSystem(Supplier.valueOf(gpSystem))
        TermsAndConditionsJourneyFactory.consent(patient, DateTime.parse("2018-11-11T00:00:00+00:00"))
    }

    @Given("^I am a patient with proof level 5 who has updated terms and conditions$")
    fun iAmAPatientWithProofLevel5WhoHasUpdatedTermsAndConditions() {
        val patient = initialisePatientAndGpSystem(Supplier.EMIS, IdentityProofingLevel.P5)
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
        termsAndConditionsPage.acceptTermsAndConditionsCheckBox.assertChecked()
    }

    @When("^I check the agree to cookies checkbox$")
    fun iCheckTheAgreeToCookiesCheckbox() {
        termsAndConditionsPage.acceptCookiesCheckBox.click()
        termsAndConditionsPage.acceptCookiesCheckBox.assertChecked()
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

    private fun initialisePatientAndGpSystem(gpSystem: Supplier, proofLevel: IdentityProofingLevel? = null): Patient {
        var patient = Patient.getDefault(gpSystem)
        if (proofLevel != null) {
            patient = patient.copy(identityProofingLevel = proofLevel)
        }

        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem).createFor(patient)

        return patient
    }
}
