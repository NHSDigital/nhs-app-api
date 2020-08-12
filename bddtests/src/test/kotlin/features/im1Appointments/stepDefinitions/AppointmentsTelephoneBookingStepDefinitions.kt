package features.im1Appointments.stepDefinitions

import constants.Supplier
import io.cucumber.datatable.DataTable
import io.cucumber.java.en.Given
import features.im1Appointments.steps.AppointmentSerenityHelpers
import features.myrecord.factories.DemographicsFactory
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import models.Patient
import utils.SerenityHelpers
import utils.set
import utils.toMap

class AppointmentsTelephoneBookingStepDefinitions {

    @Given("I wish to book a (.*) telephone appointment$")
    fun iWishToBookATelephoneAppointment(gpSystem: String, parameters: DataTable) {
        val supplier = Supplier.valueOf(gpSystem)
        val config = AppointmentConfiguration.fromMappings(parameters.toMap())
        val patient = createPatientSetup(gpSystem, config)
        val targetTelephoneNumber = getTargetTelephoneNumber(patient, config)

        val bookingFactory = AppointmentsBookingFactory.getForSupplier(supplier)
        bookingFactory.generateAvailableSlotExampleIncludingTelephoneAppointment(
                reasonNecessityOption = config.reasonNecessity)
        bookingFactory.telephoneAppointmentBookingSetupWithResult(targetTelephoneNumber, !config.enterSymptoms)
        { builder ->
            builder
                    .respondWithSuccess()
                    .inScenario("Appointments")
                    .willSetStateTo("Appointment Booked")
        }
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponseOnceBooked()
    }

    private fun getTargetTelephoneNumber(patient: Patient, config: AppointmentConfiguration): String {
        val targetTelephoneNumber = if (config.inputType == TelephoneInputType.Select)
            patient.contactDetails.telephoneFirst else "777777777"
        AppointmentSerenityHelpers.TELEPHONE_NUMBER_TO_BOOK_AGAINST.set(targetTelephoneNumber)
        return targetTelephoneNumber
    }

    private fun createPatientSetup(gpSystem: String, config: AppointmentConfiguration): Patient {
        val supplier = Supplier.valueOf(gpSystem)
        val defaultPatient = Patient.getDefault(supplier)
        val patient = defaultPatient.copy(
                contactDetails = defaultPatient.contactDetails.copy(
                        telephoneFirst = config.firstNumber,
                        telephoneSecond = config.secondNumber))
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient, false)
        CitizenIdSessionCreateJourney().createFor(patient)
        DemographicsFactory.getForSupplier(supplier).enabled(patient)
        return patient
    }
}
