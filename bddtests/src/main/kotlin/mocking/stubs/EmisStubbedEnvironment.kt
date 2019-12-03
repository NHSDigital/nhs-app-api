package mocking.stubs

import constants.Supplier
import mocking.MockingClient
import mocking.stubs.EmisStubsPatientFactory.Companion.EMISPatientList
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.appointments.AppointmentSlotsStubs
import mocking.stubs.appointments.BookAppoinmentStubs
import mocking.stubs.appointments.CancelAppointmentsStubs
import mocking.stubs.appointments.ViewAppointmentsStubs
import mocking.stubs.myMedicalRecord.MedicalRecordStubs
import mocking.stubs.patientPracticeMessaging.PatientPracticeConversationStubs
import mocking.stubs.patientPracticeMessaging.PatientPracticeMessagingStubs
import mocking.stubs.pds.ViewSpinePdsStubs
import mocking.stubs.prescriptions.OrderRepeatPrescriptionsStubs
import mocking.stubs.prescriptions.ViewCoursesStubs
import mocking.stubs.prescriptions.ViewPrescriptionsStubs

class EmisStubbedEnvironment(private val mockingClient: MockingClient) {

    private val gpSupplier = Supplier.EMIS

    fun generateStubs() {
        mockingClient.clearWiremock()
        mockingClient.favicon()
        PatientDataGenerator.generatePatientData(EMISPatientList, gpSupplier)
        generateAppointmentStubs()
        generateMyMedicalRecordsStubs()
        generatePrescriptionStubs()
        generateSpineStubs()
        generatePatientPracticeMessagingStubs()
    }

    private fun generateAppointmentStubs() {
        ViewAppointmentsStubs(mockingClient).generateStubs(gpSupplier)
        AppointmentSlotsStubs(mockingClient).generateStubs(gpSupplier)
        BookAppoinmentStubs(mockingClient, goodPatientEMIS).generateStubs(gpSupplier)
        CancelAppointmentsStubs(mockingClient, goodPatientEMIS).generateStubs(gpSupplier)
    }

    private fun generateMyMedicalRecordsStubs() {
        MedicalRecordStubs(mockingClient).generateStubs(gpSupplier)
    }

    private fun generatePatientPracticeMessagingStubs() {
        PatientPracticeMessagingStubs(mockingClient).generateEMISStubs()
        PatientPracticeConversationStubs(mockingClient).generateEMISStubs()
    }

    private fun generatePrescriptionStubs() {

        ViewPrescriptionsStubs(mockingClient).generateStubs(gpSupplier)
        val loadEMISCourses = ViewCoursesStubs(mockingClient).coursesLoaderEMIS()
        ViewCoursesStubs(mockingClient).generateEMISStubs(loadEMISCourses)

        val courseListForOrderingPrescription = loadEMISCourses[0].medicationCourseGuid
        val uuids: MutableList<String> = mutableListOf()
        uuids.add(courseListForOrderingPrescription)

        OrderRepeatPrescriptionsStubs(goodPatientEMIS, mockingClient, uuids).generateStubs(gpSupplier)
    }

    private fun generateSpineStubs() {
        ViewPrescriptionsStubs(mockingClient).generateSpineStubs()
        ViewSpinePdsStubs(mockingClient).generateSpineStubs()
    }
}