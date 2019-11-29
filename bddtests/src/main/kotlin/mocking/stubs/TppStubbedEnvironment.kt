package mocking.stubs

import constants.Supplier
import mocking.MockingClient
import mocking.stubs.TppStubsPatientFactory.Companion.TppPatientList
import mocking.stubs.myMedicalRecord.MedicalRecordStubs
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP
import mocking.stubs.prescriptions.OrderRepeatPrescriptionsStubs
import mocking.stubs.prescriptions.ViewPrescriptionsStubs
import mocking.stubs.appointments.ViewAppointmentsStubs
import mocking.stubs.appointments.CancelAppointmentsStubs
import mocking.stubs.appointments.AppointmentSlotsStubs
import mocking.stubs.appointments.BookAppoinmentStubs

class TppStubbedEnvironment(private val mockingClient : MockingClient){

    private val gpSupplier = Supplier.TPP

    fun generateStubs() {
        PatientDataGenerator.generatePatientData(TppPatientList, gpSupplier)
        generateAppointmentStubs()
        generateMedicalRecordStubs()
        generatePrescriptionStubs()
    }

    private fun generateAppointmentStubs() {
        AppointmentSlotsStubs(mockingClient).generateStubs(gpSupplier)
        BookAppoinmentStubs(mockingClient).generateStubs(gpSupplier)
        ViewAppointmentsStubs(mockingClient).generateStubs(gpSupplier)
        CancelAppointmentsStubs(mockingClient).generateStubs(gpSupplier)
    }

    private fun generateMedicalRecordStubs() {
        MedicalRecordStubs(mockingClient).generateStubs(gpSupplier)
    }
    private fun generatePrescriptionStubs(){
        ViewPrescriptionsStubs(mockingClient).generateStubs(gpSupplier)
        OrderRepeatPrescriptionsStubs(goodPatientTPP, mockingClient).generateStubs(gpSupplier)
    }
}
