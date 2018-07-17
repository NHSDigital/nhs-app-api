package features.appointments.stepDefinitions.factories

import mocking.MockingClient
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
}