package features.appointmentHub.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.factories.DemographicsFactory
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.patients.EmisPatients
import pages.appointments.HospitalAppointmentsPage
import utils.SerenityHelpers

class HospitalAppointmentStepDefinitions {

    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage

    @Given("^I am a user who can manage their hospital appointments$")
    fun iAmAUserWhoCanManageTheirHospitalAppointments(){
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }

    @Given("^I am a user who can manage their cie hospital appointments only$")
    fun iAmAUserWhoCanManageTheirCieHospitalAppointments(){
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_CIE)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }

    @Given("^I am a user who can manage their pkb hospital appointments only$")
    fun iAmAUserWhoCanManageTheirPkbHospitalAppointments(){
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }

    @Given("^I am a user who can manage their ers hospital appointments only$")
    fun iAmAUserWhoCanManageTheirErsHospitalAppointments(){
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_PKB)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }

    @Given("^I am a user who can manage their hospital appointments and has linked profiles$")
    fun iAmAUserWhoCanManageTheirHospitalAppointmentsAndHasLinkedProfiles() {
        val patient = EmisPatients.getPatientWithLinkedProfiles()
        val supplier = Supplier.EMIS
        val journey = SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB
        val odsCode = ServiceJourneyRulesMapper.findOdsCode(supplier, arrayListOf(journey))
        patient.updateOdsCodes(odsCode)
        SerenityHelpers.setGpSupplier(supplier)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory
                .getForSupplier(supplier)
                .createFor(patient)
        DemographicsFactory
                .getForSupplier(supplier)
                .enableForPatientProxyAccounts(patient)
    }

    @Given("^I am a user without the permission to manage their hospital appointments$")
    fun iAmAUserWithoutThePermissionToManageTheirHospitalAppointments(){
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_NONE)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }

    @Then("^the Hospital Appointments page is displayed$")
    fun assertIsDisplayed() {
        hospitalAppointmentsPage.assertPageTitleIsDisplayed()
    }

    @Then("^the Hospital Appointments links are displayed$")
    fun theHospitalAppointmentsPageLinksAreDisplayed() {
        hospitalAppointmentsPage.assertPageTitleIsDisplayed()
    }
}
