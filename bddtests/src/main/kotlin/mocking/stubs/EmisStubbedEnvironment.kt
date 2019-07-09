package mocking.stubs

import mocking.MockingClient
import mocking.stubs.EmisStubsPatientFactory.Companion.EMISPatientList
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.appointments.AppointmentSlotsStubs
import mocking.stubs.appointments.BookAppoinmentStubs
import mocking.stubs.appointments.CancelAppointmentsStubs
import mocking.stubs.appointments.ViewAppointmentsStubs
import mocking.stubs.myMedicalRecord.AllergiesStubs
import mocking.stubs.myMedicalRecord.ConsultationsStubs
import mocking.stubs.myMedicalRecord.ImmunisationsStubs
import mocking.stubs.myMedicalRecord.MedicationsStubs
import mocking.stubs.myMedicalRecord.ProblemsStubs
import mocking.stubs.myMedicalRecord.TestResultsStubs
import mocking.stubs.prescriptions.OrderRepeatPrescriptionsStubs
import mocking.stubs.prescriptions.ViewCoursesStubs
import mocking.stubs.prescriptions.ViewPrescriptionsStubs
import mocking.stubs.pds.ViewSpinePdsStubs

class EmisStubbedEnvironment(private val mockingClient: MockingClient) {

    fun generateStubs() {
        mockingClient.clearWiremock()
        mockingClient.favicon()
        PatientDataGenerator.generatePatientData(EMISPatientList, "EMIS")
        generateAppointmentStubs()
        generateMyMedicalRecordsStubs()
        generatePrescriptionStubs()
        generateSpineStubs()
    }

    private fun generateAppointmentStubs() {
        ViewAppointmentsStubs(mockingClient).generateEMISStubs()
        AppointmentSlotsStubs(mockingClient).generateEMISStubs()
        BookAppoinmentStubs(goodPatientEMIS, mockingClient).generateEMISStubs()
        CancelAppointmentsStubs(goodPatientEMIS, mockingClient).generateEMISStubs()
    }

    private fun generateMyMedicalRecordsStubs() {
        TestResultsStubs(mockingClient).generateEMISStubs()
        ImmunisationsStubs(mockingClient).generateEMISStubs()
        AllergiesStubs(mockingClient).generateEMISStubs()
        MedicationsStubs(mockingClient).generateEMISStubs()
        ConsultationsStubs(mockingClient).generateEMISStubs()
        ProblemsStubs(mockingClient).generateEMISStubs()
    }

    private fun generatePrescriptionStubs() {

        ViewPrescriptionsStubs(mockingClient).generateEMISStubs()
        val loadEMISCourses = ViewCoursesStubs(mockingClient).coursesLoaderEMIS()
        ViewCoursesStubs(mockingClient).generateEMISStubs(loadEMISCourses)

        val courseListForOrderingPrescription = loadEMISCourses[0].medicationCourseGuid
        val uuids: MutableList<String> = mutableListOf()
        uuids.add(courseListForOrderingPrescription)

        OrderRepeatPrescriptionsStubs(goodPatientEMIS, uuids, mockingClient).generateEMISStubs()
    }

    private fun generateSpineStubs() {
        ViewPrescriptionsStubs(mockingClient).generateSpineStubs()
        ViewSpinePdsStubs(mockingClient).generateSpineStubs()
    }
}