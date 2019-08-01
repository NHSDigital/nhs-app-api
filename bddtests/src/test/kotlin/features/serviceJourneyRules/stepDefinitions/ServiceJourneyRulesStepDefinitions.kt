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
import worker.models.serviceJourneyRules.MedicalRecordProvider
import worker.models.serviceJourneyRules.PrescriptionsProvider
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse
import java.util.*

private const val EMIS_GP_SUPPLIER = "EMIS"
private const val ODSCODE_IM1_ECONSULT_NOMINATED_PHARMACY_ENABLED = "A11111"
private const val ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED = "A22222"
private const val ODSCODE_GP_AT_HAND_CONFIGURATIONS = "A44444"

class ServiceJourneyRulesStepDefinitions {

    private val journeysToGpInformationMap = mapOf(
            EnumSet.of(JourneyType.APPOINTMENTS_IM1,
                    JourneyType.MEDICAL_RECORD_IM1,
                    JourneyType.PRESCRIPTIONS_IM1,
                    JourneyType.NOMINATED_PHARMACY_ENABLED)
                    to GpInformation(EMIS_GP_SUPPLIER, ODSCODE_IM1_ECONSULT_NOMINATED_PHARMACY_ENABLED),
            EnumSet.of(JourneyType.APPOINTMENTS_INFORMATICA,
                    JourneyType.MEDICAL_RECORD_IM1,
                    JourneyType.PRESCRIPTIONS_IM1,
                    JourneyType.NOMINATED_PHARMACY_DISABLED)
                    to GpInformation(EMIS_GP_SUPPLIER,  ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED),
            EnumSet.of(JourneyType.APPOINTMENTS_GPATHAND,
                    JourneyType.MEDICAL_RECORD_GPATHAND,
                    JourneyType.PRESCRIPTIONS_GPATHAND)
                    to GpInformation(EMIS_GP_SUPPLIER, ODSCODE_GP_AT_HAND_CONFIGURATIONS)
    )

    @Given("^I am a user whose ODS Code does not have specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeDoesNotHaveASpecificJourneyConfigurationSetUp() {
        SerenityHelpers.setGpSupplier("TPP")

        val patient = Patient.getDefault("TPP")
                .copy(odsCode = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        patient.tppUserSession!!.copy(unitId = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        SerenityHelpers.setPatient(patient)
    }

    @Given("^I am a user where the journey configurations are:$")
    fun iAmAUserWhereTheJourneyConfigurationsAre(configurations: List<Configuration>) {
        val journeyTypes =
                configurations.map { configuration -> configuration.toJourneyType() }
        val gpInformation = findGpInformation(journeyTypes)

        Assert.assertNotNull("Test setup incorrect: Cannot find a matching ODScode with given configuration in SJR",
                gpInformation)

        val patient = Patient.getDefault(gpInformation!!.gpSupplier).copy(odsCode = gpInformation.odsCode)

        SerenityHelpers.setGpSupplier(gpInformation.gpSupplier)
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpInformation.gpSupplier, mockingClient).createFor(patient)
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

    private fun findGpInformation(journeyTypes: Collection<JourneyType>): GpInformation? {
        journeysToGpInformationMap.forEach { (journeyTypesConfig, gpInformation) ->
            if (journeyTypesConfig.size >= journeyTypes.size && journeyTypesConfig.containsAll(journeyTypes)) {
                return gpInformation
            }
        }
        return null
    }

    class Configuration(val journey: String, val value: String) {

        fun toJourneyType(): JourneyType {
            val journeyType = "${journey}_$value".replace(" ", "_").toUpperCase()

            Assert.assertTrue("Test setup incorrect, journey `$journey` does not contain value for `$value`",
                    enumValues<JourneyType>().any { it.name == journeyType })

            return JourneyType.valueOf(journeyType)
        }
    }

    data class GpInformation(val gpSupplier: String, val odsCode: String)

    enum class JourneyType {
        APPOINTMENTS_GPATHAND,
        APPOINTMENTS_IM1,
        APPOINTMENTS_INFORMATICA,
        MEDICAL_RECORD_GPATHAND,
        MEDICAL_RECORD_IM1,
        NOMINATED_PHARMACY_DISABLED,
        NOMINATED_PHARMACY_ENABLED,
        PRESCRIPTIONS_GPATHAND,
        PRESCRIPTIONS_IM1
    }
}
