package features.gpSessionOnDemand.stepDefinitions

import constants.Supplier
import features.authentication.stepDefinitions.AuthenticationFactory
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.termsAndConditions.TermsAndConditionsJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.session.UserSessionRequest

class GpSessionOnDemandStepDefinitions {

    private val mockingClient = MockingClient.instance

    @Then("^NHS Login returns an invalid Subject upon establishing a (.*) GP session$")
    fun nhsLoginReturnsAnInvalidSubjectUponEstablishingAGPSession(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
            "NHS_LOGIN_PATIENT_SUBJECT_OVERRIDE", "invalid-${Patient.getDefault(supplier).subject}")
    }

    @Given("^I have a valid GP Session$")
    fun iHaveAValidGpSession() {
        val patient = SerenityHelpers.getPatient()
        val ssoRedirectUri = GlobalSerenityHelpers.GP_SESSION_REDIRECT_URI.getOrFail<String>()
        Assert.assertNotNull(Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode =patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = ssoRedirectUri)))
    }

    @Given("^I am a patient who does not have care plans and the GP System is unavailable$")
    fun iAmAPatientWhoDoesNotHaveCarePlans() {
        initialisePatientWithPkbBrandUnavailableGpSystem(
            SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE)
    }

    @Given("^I am a patient who does not have health tracker and the GP System is unavailable$")
    fun iAmAPatientWhoDoesNotHaveHealthTracker() {
        initialisePatientWithPkbBrandUnavailableGpSystem(
            SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE)
    }

    @Given("^I am a patient with pkb care plans and the GP System is unavailable$")
    fun patientWithPkbCarePlansUnavailableGpSystem() {
        initialisePatientWithPkbBrandUnavailableGpSystem(
            SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB)
    }

    @Given("^I am a patient with pkb health tracker and the GP System is unavailable$")
    fun patientWithPkbHealthTrackerUnavailableGpSystem() {
        initialisePatientWithPkbBrandUnavailableGpSystem(
            SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB)
    }

    private fun initialisePatientWithPkbBrandUnavailableGpSystem(journey: SJRJourneyType) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
            null, arrayListOf(SJRJourneyType.MEDICAL_RECORD_IM1, journey))
        val supplier = SerenityHelpers.getGpSupplier()
        setupPatient(patient, supplier)

        AuthenticationFactory.getForSupplier(supplier)
            .validOAuthDetailsAndGpSystemUnavailable(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    private fun setupPatient(patient: Patient, supplier: Supplier) {
        mockingClient.favicon()

        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
