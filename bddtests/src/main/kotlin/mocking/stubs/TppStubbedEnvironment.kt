package mocking.stubs

import mocking.MockingClient
import mocking.stubs.TppStubsPatientFactory.Companion.TppPatientList
import mocking.stubs.myMedicalRecord.MedicalRecordStubs
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP
import mocking.stubs.prescriptions.OrderRepeatPrescriptionsStubs
import mocking.stubs.prescriptions.ViewPrescriptionsStubs
import mocking.stubs.appointments.ViewAppointmentStubsTPP
import mocking.stubs.appointments.CancelAppointmentsStubs
import mocking.stubs.appointments.AppointmentSlotsStubs
import mocking.stubs.appointments.BookAppoinmentStubs

private const val GP_SUPPLIER = "TPP"
class TppStubbedEnvironment(private val mockingClient : MockingClient){
    fun generateStubs() {
        generateAppointmentStubs()
        PatientDataGenerator.generatePatientData(TppPatientList, GP_SUPPLIER)
        generateMedicalRecordStubs()
        generatePrescriptionStubs()
        PatientDataGenerator.generatePatientData(TppPatientList, GP_SUPPLIER)
    }

    private fun generateAppointmentStubs() {
        AppointmentSlotsStubs(mockingClient).generateStubs(GP_SUPPLIER)
        BookAppoinmentStubs(mockingClient).generateStubs(GP_SUPPLIER)
        ViewAppointmentStubsTPP().createHistoricalAndUpcomingAppointmentsTPP(GP_SUPPLIER)
        CancelAppointmentsStubs(mockingClient).generateStubs(GP_SUPPLIER)
    }

    private fun generateMedicalRecordStubs() {
        MedicalRecordStubs(mockingClient).generateStubs(GP_SUPPLIER)
    }
    private fun generatePrescriptionStubs(){
        ViewPrescriptionsStubs(mockingClient).generateStubs(GP_SUPPLIER)
        OrderRepeatPrescriptionsStubs(goodPatientTPP, mockingClient).generateStubs(GP_SUPPLIER)
    }
}
