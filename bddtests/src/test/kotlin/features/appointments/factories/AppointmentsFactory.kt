package features.appointments.factories

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity

abstract class AppointmentsFactory(gpSupplier:String){

    val mockingClient = MockingClient.instance
    var patient : Patient = Patient.getDefault(gpSupplier)
    protected var supplier :String = gpSupplier
    protected var appointmentMapper: MockingClientAppointmentMappingFactory

    init{
        Serenity.setSessionVariable(Patient::class).to(patient)
        appointmentMapper = MockingClientAppointmentMappingFactory.getForSupplier(supplier)
    }


    protected fun generateDefaultUserData() {

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        createGetEmptyAppointmentList()
    }

    private fun createGetEmptyAppointmentList() {
        val viewAppointmentFactory = ViewAppointmentsFactory.getForSupplier(supplier)
        val getResponse = viewAppointmentFactory.createEmptyUpcomingAppointmentResponse(patient)
        appointmentMapper
                .requestMapping{ viewMyAppointmentsRequest(patient).respondWithSuccess(getResponse)}
    }
}