package mocking.stubs

import mocking.stubs.TppStubsPatientFactory.Companion.TppPatientList
import mocking.stubs.appointments.ViewAppointmentStubs

private const val GP_SUPPLIER = "TPP"
class TppStubbedEnvironment{
    fun generateStubs() {
        generateAppointmentStubs()
        PatientDataGenerator.generatePatientData(TppPatientList, GP_SUPPLIER)
    }

    private fun generateAppointmentStubs() {
        ViewAppointmentStubs().createHistoricalUpcomingAppointments(GP_SUPPLIER)
    }
}
