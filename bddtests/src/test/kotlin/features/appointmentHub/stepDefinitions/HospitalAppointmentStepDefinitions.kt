package features.appointmentHub.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.factories.DemographicsFactory
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import models.patients.EmisPatients
import pages.appointments.HospitalAppointmentsPage
import utils.SerenityHelpers

class HospitalAppointmentStepDefinitions {

    private val mockingClient = MockingClient.instance

    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage

    @Given("^I am a user who can manage their hospital appointments$")
    fun iAmAUserWhoCanManageTheirHospitalAppointments(){
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
    }

    @Given("^I am a user who can manage their hospital appointments and has linked profiles$")
    fun iAmAUserWhoCanManageTheirHospitalAppointmentsAndHasLinkedProfiles() {
        val patient = EmisPatients.getPatientWithLinkedProfiles()
        val supplier = Supplier.EMIS
        val journey = ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB
        val gpInformation = ServiceJourneyRulesMapper.findGpInformation(
                null, arrayListOf(journey))
        Patient.updateOdsCodes(patient, gpInformation!!.odsCode)
        SerenityHelpers.setGpSupplier(supplier)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory
                .getForSupplier(supplier, mockingClient)
                .createFor(patient)
        DemographicsFactory
                .getForSupplier(supplier)
                .enableForPatientProxyAccounts(patient)
    }

    @Given("^I am a user without the permission to manage their hospital appointments$")
    fun iAmAUserWithoutThePermissionToManageTheirHospitalAppointments(){
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_NONE)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
    }

    @Then("^the Hospital Appointments page is displayed$")
    fun assertIsDisplayed() {
        hospitalAppointmentsPage.assertIsDisplayed()
    }
}
