package features.serviceJourneyRules.stepDefinitions

import com.google.gson.internal.LazilyParsedNumber
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.backend.CommonSteps
import mocking.defaults.TppMockDefaults.Companion.TPP_ODS_CODE_NO_SJR_CONFIGURATION
import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.serviceJourneyRules.AppointmentsProvider
import worker.models.serviceJourneyRules.MedicalRecordProvider
import worker.models.serviceJourneyRules.PrescriptionsProvider
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse

private const val TPP_GP_SUPPLIER = "TPP"

class ServiceJourneyRulesStepDefinitions {

    @Given("^I am a user whose ODS Code does not have specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeDoesNotHaveASpecificJourneyConfigurationSetUp() {
        SerenityHelpers.setGpSupplier(TPP_GP_SUPPLIER)

        val patient = Patient.getDefault(TPP_GP_SUPPLIER)
                .copy(odsCode = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        patient.tppUserSession!!.copy(unitId = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        SerenityHelpers.setPatient(patient)
    }

    @Given("^I am a (.*) user where the journey configurations are:$")
    fun iAmAGPSystemUserWhereTheJourneyConfigurationsAre(gpSystem: String,
                                                 configurations: List<ServiceJourneyRulesConfiguration>) {
        createUser(gpSystem, configurations)
    }

    @Given("^I am a user where the journey configurations are:$")
    fun iAmAUserWhereTheJourneyConfigurationsAre(configurations: List<ServiceJourneyRulesConfiguration>) {
        createUser(null, configurations)
    }

    private fun createUser(gpSystem: String?,
                           configurations: List<ServiceJourneyRulesConfiguration>) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(gpSystem, ArrayList(configurations))
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(
                gpSystem ?: SerenityHelpers.getGpSupplier(),
                mockingClient).createFor(patient)
    }

    @When("^I request the service journey rules for my ODS Code$")
    fun iRequestTheServiceJourneyRulesForODSCode() {
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .serviceJourneyRules
                    .getServiceJourneyRulesConfiguration()

            ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE.set(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I login but service journey rules has no configuration for my GP practice")
    fun whenILoginButServiceJourneyRulesHasNoConfigurationForMyGPPractice() {
        try {
            CommonSteps().givenIHaveLoggedInAndHaveAValidSessionCookie()
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^the service journey rules response will have appointments set to (\\w+)$")
    fun theServiceJourneyRulesResponseWillHaveAppointmentsSetTo(provider: String) {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Appointments provider",
                AppointmentsProvider.valueOf(provider),
                serviceJourneyRulesResponse.journeys.appointments.provider)
    }

    @Then("^the service journey rules response will have medical record set to (\\w+)$")
    fun theServiceJourneyRulesResponseWillHaveMedicalRecordSetTo(provider: String){
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()
        Assert.assertNotNull(
                "Service Journey Rules response expected, but was null",
                serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Medical Record provider",
                MedicalRecordProvider.valueOf(provider),
                serviceJourneyRulesResponse.journeys.medicalRecord.provider)
    }

    @Then("^the service journey rules response will have medical record version set to (\\d+)$")
    fun theServiceJourneyRulesResponseWillHaveMedicalRecordVersionSetTo(version: String) {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()
        Assert.assertNotNull(
                "Service Journey Rules response expected, but was null",
                serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Medical Record version",
                LazilyParsedNumber(version),
                serviceJourneyRulesResponse.journeys.medicalRecord.version)
    }

    @Then("^the service journey rules response will have prescriptions set to (\\w+)$")
    fun theServiceJourneyRulesResponseWillHavePrescriptionsSetTo(provider: String){
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Prescriptions provider",
                PrescriptionsProvider.valueOf(provider),
                serviceJourneyRulesResponse.journeys.prescriptions.provider)
    }

    @Then("^the service journey rules response will have nominated pharmacy (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveNominatedPharmacyEnabledOrDisabled(enabled: String) {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules nominated pharmacy provider",
                enabled == "enabled",
                serviceJourneyRulesResponse.journeys.nominatedPharmacy)
    }
}
