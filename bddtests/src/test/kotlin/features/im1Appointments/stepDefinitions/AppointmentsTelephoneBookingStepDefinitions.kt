package features.im1Appointments.stepDefinitions

import cucumber.api.DataTable
import cucumber.api.java.en.Given
import features.im1Appointments.steps.AppointmentSerenityHelpers
import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import models.Patient
import utils.SerenityHelpers
import utils.set
import utils.toMap

class AppointmentsTelephoneBookingStepDefinitions {

    val mockingClient = MockingClient.instance

    @Given("I wish to book a (.*) telephone appointment$")
    fun iWishToBookATelephoneAppointment(gpSystem: String, parameters: DataTable) {
        val config = AppointmentConfiguration.fromMappings(parameters.toMap())
        val patient = createPatientSetup(gpSystem, config)
        val targetTelephoneNumber = getTargetTelephoneNumber(patient, config)

        val bookingFactory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        bookingFactory.generateAvailableSlotExampleIncludingTelephoneAppointment(
                reasonNecessityOption = config.reasonNecessity)
        bookingFactory.telephoneAppointmentBookingSetupWithResult(targetTelephoneNumber, !config.enterSymptoms)
        { builder ->
            builder
                    .respondWithSuccess()
                    .inScenario("Appointments")
                    .willSetStateTo("Appointment Booked")
        }
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpSystem)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponseOnceBooked()
    }

    private fun getTargetTelephoneNumber(patient: Patient, config: AppointmentConfiguration): String {
        val targetTelephoneNumber = if (config.inputType == TelephoneInputType.Select)
            patient.telephoneFirst else "777777777"
        AppointmentSerenityHelpers.TELEPHONE_NUMBER_TO_BOOK_AGAINST.set(targetTelephoneNumber)
        return targetTelephoneNumber
    }

    private fun createPatientSetup(gpSystem: String, config: AppointmentConfiguration): Patient {
        val patient = Patient.getDefault(gpSystem).copy(
                telephoneFirst = config.firstNumber,
                telephoneSecond = config.secondNumber)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient, false)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
        return patient
    }
}
