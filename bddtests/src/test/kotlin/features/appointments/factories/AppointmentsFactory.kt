package features.appointments.factories

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity.setSessionVariable
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

abstract class AppointmentsFactory(gpSupplier: String) {

    val mockingClient = MockingClient.instance
    var patient: Patient = Patient.getDefault(gpSupplier)
    protected var supplier: String = gpSupplier
    protected var appointmentMapper: MockingClientAppointmentMappingFactory

    private var tomorrowDate = LocalDateTime.now().plusDays(1)

    init {
        setSessionVariable(Patient::class).to(patient)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }

    fun generateDefaultUserData() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        createGetEmptyAppointmentList()
    }

    private fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = ViewAppointmentsFactory.getForSupplier(supplier)
        val getResponse = viewAppointmentFactory.createEmptyUpcomingAppointmentResponse(patient)
        appointmentMapper
                .requestMapping { viewMyAppointmentsRequest(patient).respondWithSuccess(getResponse) }
    }

    protected fun storeDateAndTimeOfExpectedSlotAsPerUI() {
        //Format like : Wednesday 1 August 2018
        val formatter = DateTimeFormatter.ofPattern("EEEE d MMMM yyyy")
        val day = tomorrowDate.format(formatter)
        setSessionVariable(TargetAppointmentDateKey).to(day)
        setSessionVariable(TargetAppointmentTimeKey).to("2:00pm")
    }

    companion object {
        const val AppointmentStartTimeKey = "AppointmentStartTimeKey"
        const val AppointmentEndTimeKey = "AppointmentEndTimeKey"

        const val TargetAppointmentDateKey = "TargetAppointmentDateKey"
        const val TargetAppointmentTimeKey = "TargetAppointmentTimeKey"
    }
}