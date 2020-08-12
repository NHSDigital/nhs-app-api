package features.serviceJourneyRules.stepDefinitions

import com.google.gson.internal.LazilyParsedNumber
import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.serviceJourneyRules.mappers.PublicHealthNotificationsMapper
import mocking.defaults.TppMockDefaults.Companion.TPP_ODS_CODE_NO_SJR_CONFIGURATION
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.serviceJourneyRules.AppointmentsProvider
import worker.models.serviceJourneyRules.ConsultationsProvider
import worker.models.serviceJourneyRules.MedicalRecordProvider
import worker.models.serviceJourneyRules.MessagesProvider
import worker.models.serviceJourneyRules.PrescriptionsProvider
import worker.models.serviceJourneyRules.SecondaryAppointmentsProvider
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse
import worker.models.session.UserSessionRequest

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

    @Given("^I am a (.*) user where the expected journey configurations are:$")
    fun iAmAGPSystemUserWhereTheExpectedJourneyConfigurationsAre(gpSystem: String,
                                                         configurations: List<ServiceJourneyRulesConfiguration>) {
        iAmAGPSystemUserWhereTheJourneyConfigurationsAre(gpSystem, configurations)
        PublicHealthNotificationsMapper.setSerenityVariablesForJourneys(
                SerenityHelpers.getPatient().odsCode,
                configurations)
    }

    @Given("^I am a user where the journey configurations are:$")
    fun iAmAUserWhereTheJourneyConfigurationsAre(configurations: List<ServiceJourneyRulesConfiguration>) {
        createUser(null, configurations)
    }

    private fun createUser(supplier: Supplier?,
                           configurations: List<ServiceJourneyRulesConfiguration>) {
        val journeyTypes = configurations.map { configuration -> configuration.toJourneyType() }
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(supplier, journeyTypes)
        val supplierToUse = supplier ?: SerenityHelpers.getGpSupplier()
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(
                supplierToUse).createFor(patient)
    }

    @When("^I request the service journey rules for my ODS Code$")
    fun iRequestTheServiceJourneyRulesForODSCode() {
        val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .serviceJourneyRules
                .getServiceJourneyRulesConfiguration()
        ServiceJourneyRulesSerenityHelpers.SERVICE_JOURNEY_RULES_RESPONSE.set(response)
    }

    @When("^I login but service journey rules has no configuration for my GP practice")
    fun whenILoginButServiceJourneyRulesHasNoConfigurationForMyGPPractice() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem).createFor(patient)
        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode = patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = redirectUri))
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

    @Then("^the service journey rules response will have im1Messaging is enabled set to (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveim1MessagingSetTo(enabled: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val actualValue = serviceJourneyRulesResponse.journeys.im1Messaging.isEnabled
        Assert.assertEquals("Service Journey Rules im1 messaging is enabled",
                                 enabled == "enabled",
                                 actualValue)
    }

    @Then("^the service journey rules response will have im1Messaging can delete messages set to (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveim1MessagingCanDeleteSetTo(enabled: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val actualValue = serviceJourneyRulesResponse.journeys.im1Messaging.canDeleteMessages
        Assert.assertEquals("Service Journey Rules im1 messaging can delete messages",
                            enabled == "enabled",
                            actualValue)
    }

    @Then("^the service journey rules response will have im1Messaging can update messages set to (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveim1MessagingCanUpdateSetTo(enabled: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val actualValue = serviceJourneyRulesResponse.journeys.im1Messaging.canUpdateReadStatus
        Assert.assertEquals("Service Journey Rules im1 messaging can update messages",
                enabled == "enabled",
                actualValue)
    }

    @Then("^the service journey rules response will have im1Messaging required details call set to (enabled|disabled)$")
    fun theServiceJourneyRulesResponseWillHaveim1MessagingRequiresDetailsCallSetTo(enabled: String) {
        val serviceJourneyRulesResponse = getServiceJourneyRulesResponse()
        val actualValue = serviceJourneyRulesResponse.journeys.im1Messaging.requiresDetailsRequest
        Assert.assertEquals("Service Journey Rules im1 messaging requires additional call for details",
                enabled == "enabled",
                actualValue)
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

