package mocking.stubs

import mocking.MockingClient
import mocking.stubs.EmisStubsPatientFactory.Companion.EMISPatientList
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.appointments.AppointmentSlotsStubs
import mocking.stubs.appointments.BookAppoinmentStubs
import mocking.stubs.appointments.CancelAppointmentsStubs
import mocking.stubs.appointments.ViewAppointmentsStubsEMIS
import mocking.stubs.myMedicalRecord.MedicalRecordStubs
import mocking.stubs.pds.ViewSpinePdsStubs
import mocking.stubs.prescriptions.OrderRepeatPrescriptionsStubs
import mocking.stubs.prescriptions.ViewCoursesStubs
import mocking.stubs.prescriptions.ViewPrescriptionsStubs

private const val GP_SUPPLIER = "EMIS"

class EmisStubbedEnvironment(private val mockingClient: MockingClient) {

    fun generateStubs() {
        mockingClient.clearWiremock()
        mockingClient.favicon()
        PatientDataGenerator.generatePatientData(EMISPatientList, GP_SUPPLIER)
        generateAppointmentStubs()
        generateMyMedicalRecordsStubs()
        generatePrescriptionStubs()
        generateSpineStubs()
    }

    private fun generateAppointmentStubs() {
        ViewAppointmentsStubsEMIS(mockingClient).generateEMISStubs()
        AppointmentSlotsStubs(mockingClient).generateEMISStubs()
        BookAppoinmentStubs(goodPatientEMIS, mockingClient).generateEMISStubs()
        CancelAppointmentsStubs(goodPatientEMIS, mockingClient).generateEMISStubs()
    }

    private fun generateMyMedicalRecordsStubs() {
        MedicalRecordStubs(mockingClient).generateStubs(GP_SUPPLIER)
    }

    private fun generatePrescriptionStubs() {

        ViewPrescriptionsStubs(mockingClient).generateStubs(GP_SUPPLIER)
        val loadEMISCourses = ViewCoursesStubs(mockingClient).coursesLoaderEMIS()
        ViewCoursesStubs(mockingClient).generateEMISStubs(loadEMISCourses)

        val courseListForOrderingPrescription = loadEMISCourses[0].medicationCourseGuid
        val uuids: MutableList<String> = mutableListOf()
        uuids.add(courseListForOrderingPrescription)

        OrderRepeatPrescriptionsStubs(goodPatientEMIS, mockingClient, uuids).generateStubs(GP_SUPPLIER)
    }

    private fun generateSpineStubs() {
        ViewPrescriptionsStubs(mockingClient).generateSpineStubs()
        ViewSpinePdsStubs(mockingClient).generateSpineStubs()
    }
}