package mocking.stubs

import mocking.MockingClient
import mocking.stubs.TppStubsPatientFactory.Companion.TppPatientList
import mocking.stubs.appointments.ViewAppointmentStubs
import mocking.stubs.myMedicalRecord.MedicalRecordStubs

private const val GP_SUPPLIER = "TPP"
class TppStubbedEnvironment{
    fun generateStubs() {
        generateAppointmentStubs()
        PatientDataGenerator.generatePatientData(TppPatientList, GP_SUPPLIER)
        generateMedicalRecordStubs()
    }

    private fun generateAppointmentStubs() {
        ViewAppointmentStubs().createHistoricalUpcomingAppointments(GP_SUPPLIER)
    }

    private fun generateMedicalRecordStubs(){
        MedicalRecordStubs(MockingClient.instance).generateStubs(GP_SUPPLIER)
    }
}
