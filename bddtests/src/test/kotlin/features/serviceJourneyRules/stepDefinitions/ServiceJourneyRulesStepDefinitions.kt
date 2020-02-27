package features.serviceJourneyRules.stepDefinitions

import com.google.gson.internal.LazilyParsedNumber
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.defaults.TppMockDefaults.Companion.TPP_ODS_CODE_NO_SJR_CONFIGURATION
import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.sharedSteps.backend.SharedStepDefinitionsBackend
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
import worker.models.serviceJourneyRules.ConsultationsProvider
import worker.models.serviceJourneyRules.MedicalRecordProvider
import worker.models.serviceJourneyRules.MessagesProvider
import worker.models.serviceJourneyRules.PrescriptionsProvider
import worker.models.serviceJourneyRules.SecondaryAppointmentsProvider
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse

class ServiceJourneyRulesStepDefinitions {

    @Given("^I am a user whose ODS Code does not have specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeDoesNotHaveASpecificJourneyConfigurationSetUp() {
        SerenityHelpers.setGpSupplier(Supplier.TPP)

        val patient = Patient.getDefault(Supplier.TPP)
                .copy(odsCode = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        patient.tppUserSession!!.copy(unitId = TPP_ODS_CODE_NO_SJR_CONFIGURATION)
        SerenityHelpers.setPatient(patient)
    }

    @Given("^I am a (.*) user where the journey configurations are:$")
    fun iAmAGPSystemUserWhereTheJourneyConfigurationsAre(gpSystem: String,
                                                         configurations: List<ServiceJourneyRulesConfiguration>) {
        val supplier = Supplier.valueOf(gpSystem)
        createUser(supplier, configurations)
    }

    @Given("^I am a user where the journey configurations are:$")
    fun iAmAUserWhereTheJourneyConfigurationsAre(configurations: List<ServiceJourneyRulesConfiguration>) {
        createUser(null, configurations)
    }

    private fun createUser(supplier: Supplier?,
                           configurations: List<ServiceJourneyRulesConfiguration>) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(supplier, ArrayList(configurations))
        val supplierToUse = supplier ?: SerenityHelpers.getGpSupplier()
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(
                supplierToUse, mockingClient).createFor(patient)
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
            SharedStepDefinitionsBackend().givenIHaveLoggedInAndHaveAValidSessionCookie()
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^the service journey rules response will have appointments set to (\\w+)$")
    fun theServiceJourneyRulesResponseWillHaveAppointmentsSetTo(provider: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        Assert.assertEquals("Service Journey Rules Appointments provider",
                AppointmentsProvider.valueOf(provider),
                serviceJourneyRulesResponse.journeys.appointments.provider)
    }

    @Then("^the service journey rules response will have medical record set to (\\w+)$")
    fun theServiceJourneyRulesResponseWillHaveMedicalRecordSetTo(provider: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        Assert.assertEquals("Service Journey Rules Medical Record provider",
                MedicalRecordProvider.valueOf(provider),
                serviceJourneyRulesResponse.journeys.medicalRecord.provider)
    }

    @Then("^the service journey rules response will have medical record version set to (\\d+)$")
    fun theServiceJourneyRulesResponseWillHaveMedicalRecordVersionSetTo(version: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        Assert.assertEquals("Service Journey Rules Medical Record version",
                LazilyParsedNumber(version),
                serviceJourneyRulesResponse.journeys.medicalRecord.version)
    }

    @Then("^the service journey rules response will have prescriptions set to (\\w+)$")
    fun theServiceJourneyRulesResponseWillHavePrescriptionsSetTo(provider: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        Assert.assertEquals("Service Journey Rules Prescriptions provider",
                PrescriptionsProvider.valueOf(provider),
                serviceJourneyRulesResponse.journeys.prescriptions.provider)
    }

    @Then("^the service journey rules response will have nominated pharmacy (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveNominatedPharmacyEnabledOrDisabled(enabled: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        Assert.assertEquals("Service Journey Rules nominated pharmacy provider",
                enabled == "enabled",
                serviceJourneyRulesResponse.journeys.nominatedPharmacy)
    }

    @Then("^the service journey rules response will have no silver integration secondary appointments$")
    fun theServiceJourneyRulesResponseWillHaveNoSecondaryAppointments() {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val actualValues =   serviceJourneyRulesResponse.journeys.silverIntegrations.secondaryAppointments
        Assert.assertArrayEquals("Service Journey Rules secondary appointments provider",
                arrayOf(),
                actualValues.toTypedArray().sortedArray())
    }

    @Then("^the service journey rules response will have silver integration secondary appointments set to (.*)$")
    fun theServiceJourneyRulesResponseWillHaveSecondaryAppointmentsSetTo(values: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val expectedValues = values.split(",")
                .map { value -> SecondaryAppointmentsProvider.valueOf(value.trim()) }
        val actualValues =   serviceJourneyRulesResponse.journeys.silverIntegrations.secondaryAppointments
        Assert.assertArrayEquals("Service Journey Rules secondary appointments provider",
                expectedValues.toTypedArray().sortedArray(),
                actualValues.toTypedArray().sortedArray())
    }

    @Then("^the service journey rules response will have silver integration messages set to (.*)$")
    fun theServiceJourneyRulesResponseWillHaveMessagesSetTo(values: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val expectedValues = values.split(",")
                .map { value -> MessagesProvider.valueOf(value.trim()) }
        val actualValues = serviceJourneyRulesResponse.journeys.silverIntegrations.messages
        Assert.assertArrayEquals("Service Journey Rules messaging and consultations provider",
                expectedValues.toTypedArray().sortedArray(),
                actualValues.toTypedArray().sortedArray())
    }

    @Then("^the service journey rules response will have no silver integration messages$")
    fun theServiceJourneyRulesResponseWillHaveNoMessages() {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val actualValues =   serviceJourneyRulesResponse.journeys.silverIntegrations.messages
        Assert.assertArrayEquals("Service Journey Rules messages provider",
                arrayOf(),
                actualValues.toTypedArray().sortedArray())
    }

    @Then("^the service journey rules response will have silver integration consultations set to (.*)$")
    fun theServiceJourneyRulesResponseWillHaveConsultationsSetTo(values: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val expectedValues = values.split(",")
                .map { value -> ConsultationsProvider.valueOf(value.trim()) }
        val actualValues = serviceJourneyRulesResponse.journeys.silverIntegrations.consultations
        Assert.assertArrayEquals("Service Journey Rules consultations provider",
                expectedValues.toTypedArray().sortedArray(),
                actualValues.toTypedArray().sortedArray())
    }
    @Then("^the service journey rules response will have no silver integration consultations$")
    fun theServiceJourneyRulesResponseWillHaveNoConsultations() {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val actualValues =   serviceJourneyRulesResponse.journeys.silverIntegrations.consultations
        Assert.assertArrayEquals("Service Journey Rules consultations provider",
                arrayOf(),
                actualValues.toTypedArray().sortedArray())
    }

    private fun getServiceJourneyRulesResponse(): ServiceJourneyRulesResponse {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()
        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        return serviceJourneyRulesResponse
    }


    @Then("^the service journey rules response will have im1 messaging (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveIm1MessagingEnabledOrDisabled(enabled: String) {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules im1 messaging",
                enabled == "enabled",
                serviceJourneyRulesResponse.journeys.im1Messaging)
    }

    @Then("^the service journey rules response will have documents (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveDocumentsEnabledOrDisabled(enabled: String) {
        val serviceJourneyRulesResponse =
                ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE
                        .getOrFail<ServiceJourneyRulesResponse>()

        Assert.assertNotNull("Service Journey Rules response expected, but was null", serviceJourneyRulesResponse)
        Assert.assertEquals("Service Journey Rules documents",
                enabled == "enabled",
                serviceJourneyRulesResponse.journeys.documents)
    }
}

