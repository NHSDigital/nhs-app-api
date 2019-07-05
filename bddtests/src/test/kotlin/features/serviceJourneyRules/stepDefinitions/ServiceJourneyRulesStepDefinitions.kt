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
import java.util.*

const val EMIS_GP_SUPPLIER = "EMIS"
const val ODSCODE_IM1_ECONSULT_NOMINATED_PHARMACY_ENABLED = "A11111"
const val ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED = "A22222"

class ServiceJourneyRulesStepDefinitions {

    private val journeysOdsMap = mapOf(
            EnumSet.of(JourneyType.APPOINTMENTS_IM1, JourneyType.NOMINATED_PHARMACY_ENABLED)
                    to ODSCODE_IM1_ECONSULT_NOMINATED_PHARMACY_ENABLED,
            EnumSet.of(JourneyType.APPOINTMENTS_INFORMATICA, JourneyType.NOMINATED_PHARMACY_DISABLED)
                    to ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED
    )

    @Given("^I am a user whose ODS Code has a specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeHasASpecificJourneyConfigurationSetUp() {
        // EMIS is used as the default odsCode exists within gpinfo.csv
        SerenityHelpers.setGpSupplier(EMIS_GP_SUPPLIER)
    }

    @Given("^I am a user whose ODS Code does not have specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeDoesNotHaveASpecificJourneyConfigurationSetUp() {
        SerenityHelpers.setGpSupplier("TPP")

        // copying at moment until a new instance is returned per request
        val patient = Patient.getDefault("TPP")
                .copy(odsCode = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        patient.tppUserSession!!.copy(unitId = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        SerenityHelpers.setPatient(patient)
    }

    @Given("^I am a user where the journey configurations are:$")
    fun iAmAUserWhereTheJourneyConfigurationsAre(configurations: List<Configuration>) {
        val journeyTypes = mapToJourneyTypes(configurations)
        val odsCode = findOdsCode(journeyTypes)

        Assert.assertNotNull("Cannot find a matching ODS code with the given configuration in SJR", odsCode)

        // copying at moment until a new instance is returned per request
        val patient = Patient.getDefault(EMIS_GP_SUPPLIER).copy(odsCode = odsCode!!)

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

    @Then("^the service journey rules response will have appointments set to (\\w+)$")
    fun theServiceJourneyRulesResponseWillHaveAppointmentsSetToIm1(provider: String) {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Appointments provider",
                AppointmentsProvider.valueOf(provider),
                serviceJourneyRulesResponse.journeys.appointments.provider)
    }

    @Then("^the service journey rules response will have nominated pharmacy enabled$")
    fun theServiceJourneyRulesResponseWillHaveNominatedPharmacyEnabled() {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Appointments provider",
                true,
                serviceJourneyRulesResponse.journeys.nominatedPharmacy)
    }

    @Then("^the service journey rules response will have nominated pharmacy disabled$")
    fun theServiceJourneyRulesResponseWillHaveNominatedPharmacyDisabled() {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules Appointments provider",
                false,
                serviceJourneyRulesResponse.journeys.nominatedPharmacy)
    }

    private fun mapToJourneyTypes(configurations: List<Configuration>): Collection<JourneyType> {
        val journeyTypes = mutableListOf<JourneyType>()

        configurations.forEach { configuration ->
            val journey = configuration.journey
            val value = configuration.value
            val journeyType = "${journey.replace(" ", "_").toUpperCase()}_${value.toUpperCase()}"

            Assert.assertTrue("Test setup incorrect, journey `$journey` does not contain value for `$value`",
                    enumValues<JourneyType>().any { it.name == journeyType })

            journeyTypes.add(JourneyType.valueOf(journeyType))
        }

        return journeyTypes
    }

    private fun findOdsCode(journeyTypes: Collection<JourneyType>): String? {
        journeysOdsMap.forEach { (journeyTypesConfig, odsCode) ->
            if (journeyTypesConfig.size >= journeyTypes.size && journeyTypesConfig.containsAll(journeyTypes)) {
                return odsCode
            }
        }

        return null
    }

    data class Configuration(val journey: String, val value: String)

    enum class JourneyType {
        APPOINTMENTS_IM1,
        APPOINTMENTS_INFORMATICA,
        NOMINATED_PHARMACY_DISABLED,
        NOMINATED_PHARMACY_ENABLED
    }
}
