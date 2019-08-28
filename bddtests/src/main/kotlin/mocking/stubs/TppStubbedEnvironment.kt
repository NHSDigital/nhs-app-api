package mocking.stubs

import mocking.MockingClient
import mocking.stubs.TppStubsPatientFactory.Companion.TppPatientList
import mocking.stubs.appointments.ViewAppointmentStubs
import mocking.stubs.myMedicalRecord.MedicalRecordStubs
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP
import mocking.stubs.prescriptions.OrderRepeatPrescriptionsStubs
import mocking.stubs.prescriptions.ViewPrescriptionsStubs

private const val GP_SUPPLIER = "TPP"
class TppStubbedEnvironment{
    fun generateStubs() {
        generateAppointmentStubs()
        PatientDataGenerator.generatePatientData(TppPatientList, GP_SUPPLIER)
        generateMedicalRecordStubs()
        generatePrescriptionStubs()
    }

    private fun generateAppointmentStubs() {
        ViewAppointmentStubs().createHistoricalUpcomingAppointments(GP_SUPPLIER)
    }

    private fun generateMedicalRecordStubs() {
        MedicalRecordStubs(MockingClient.instance).generateStubs(GP_SUPPLIER)
    }
    private fun generatePrescriptionStubs(){
        ViewPrescriptionsStubs(MockingClient.instance).generateStubs(GP_SUPPLIER)
        OrderRepeatPrescriptionsStubs(goodPatientTPP, MockingClient.instance).generateStubs(GP_SUPPLIER)
    }
}
