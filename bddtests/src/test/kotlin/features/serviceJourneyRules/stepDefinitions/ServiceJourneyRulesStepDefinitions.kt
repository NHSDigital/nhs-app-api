package features.serviceJourneyRules.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.defaults.TppMockDefaults.Companion.TPP_ODS_CODE_NO_SJR_CONFIGURATION
import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
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
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse

const val EMIS_GP_SUPPLIER = "EMIS"
const val ODSCODE_IM1_APPOINTMENTS = "A11111"
const val ODSCODE_INFORMATICA_APPOINTMENTS = "A22222"

class ServiceJourneyRulesStepDefinitions {

    private lateinit var patient: Patient

    @Given("^I am a user whose ODS Code has a specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeHasASpecificJourneyConfigurationSetUp() {
        // EMIS is used as the default odsCode exists within gpinfo.csv
        SerenityHelpers.setGpSupplier(EMIS_GP_SUPPLIER)
    }

    @Given("^I am a user whose ODS Code does not have specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeDoesNotHaveASpecificJourneyConfigurationSetUp() {
        SerenityHelpers.setGpSupplier("TPP")
        patient = Patient.getDefault("TPP")
        patient.odsCode = TPP_ODS_CODE_NO_SJR_CONFIGURATION
        patient.tppUserSession!!.unitId = TPP_ODS_CODE_NO_SJR_CONFIGURATION
        SerenityHelpers.setPatient(patient)
    }

    @Given("^I am a user where the journey configuration for appointments is set to (\\w+)$")
    fun iAmAUserWhereTheJourneyConfigurationsForAppointmentsIsSetTo(appointmentsJourney: String) {

        val journeyTypeToODSCode = mapOf(
                "Im1" to  ODSCODE_IM1_APPOINTMENTS,
                "Informatica" to ODSCODE_INFORMATICA_APPOINTMENTS
        )

        Assert.assertTrue(
                "Test setup incorrect, map does not contain value for $appointmentsJourney",
                journeyTypeToODSCode.containsKey(appointmentsJourney))

        patient = Patient.getDefault(EMIS_GP_SUPPLIER)
        patient.odsCode = journeyTypeToODSCode.getValue(appointmentsJourney)

        SerenityHelpers.setGpSupplier(EMIS_GP_SUPPLIER)
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(EMIS_GP_SUPPLIER, mockingClient).createFor(patient)
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

    @Then("^I receive the service journey rules response$")
    fun iReceiveServiceJourneyRulesOnTheResponse() {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
    }

    @Then("^the service journey rules response will have appointments set to im1$")
    fun theServiceJourneyRulesResponseWillHaveAppointmentsSetToIm1(){
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Appointments provider",
                AppointmentsProvider.im1,
                serviceJourneyRulesResponse.journeys.appointments.provider)
    }

    @Then("^the service journey rules response will have appointments set to informatica$")
    fun theServiceJourneyRulesResponseWillHaveAppointmentsSetToInformatica(){
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Appointments provider",
                AppointmentsProvider.informatica,
                serviceJourneyRulesResponse.journeys.appointments.provider)
    }
}
