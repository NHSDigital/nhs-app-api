package mocking.stubs

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.stubs.StubsPatientFactory.Companion.EMISPatientList
import mocking.stubs.StubsPatientFactory.Companion.goodPatientEMIS
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

class StubbedEnvironment(private val mockingClient: MockingClient) {
    companion object {
        const val TIMEOUT_DELAY: Long = 71
    }

    fun generateEMISStubs() {
        mockingClient.clearWiremock()
        mockingClient.favicon()
        generatePatientData()
        generateAppointmentStubs()
        generateMyMedicalRecordsStubs()
        generatePrescriptionStubs()
    }

    private fun generatePatientData() {
        EMISPatientList.forEach { patientDetails ->
            CitizenIdStubs(mockingClient).createFor(patientDetails)
            EmisSessionCreateJourneyFactory(mockingClient).createFor(patientDetails)
        }
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
}