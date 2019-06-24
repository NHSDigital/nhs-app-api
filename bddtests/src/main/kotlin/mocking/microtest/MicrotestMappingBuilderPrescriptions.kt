package mocking.microtest

import mocking.microtest.prescriptions.PrescriptionRequestPost

import models.Patient
import java.time.OffsetDateTime

class MicrotestMappingBuilderPrescriptions {

    fun getPrescriptionHistoryRequest(patient: Patient, fromDate: OffsetDateTime? = null) =
            PrescriptionsBuilderMicrotest(
                    patient.nhsNumbers.first(),
                    patient.odsCode,
                    fromDate)

    fun getCoursesRequest(patient: Patient) = CoursesBuilderMicrotest(
            patient.nhsNumbers.first(),
            patient.odsCode)

    fun repeatPrescriptionSubmissionRequest(patient: Patient,
                                            prescriptionRequestsPost: PrescriptionRequestPost? = null) =
            PrescriptionSubmissionBuilderMicrotest(
                    patient.nhsNumbers.first(),
                    patient.odsCode,
                    prescriptionRequestsPost)
}
