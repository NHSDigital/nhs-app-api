package mocking.emis

import mocking.emis.courses.EmisCoursesBuilder
import mocking.emis.prescriptions.EmisPrescriptionsBuilder
import mocking.emis.prescriptionsSubmission.EmisPrescriptionsSubmissionBuilder
import models.Patient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.OffsetDateTime

class EmisMappingBuilderPrescriptions(private var configuration: EmisConfiguration?){

    fun prescriptionsRequest(patient: Patient, fromDate: OffsetDateTime? = null,
                             toDate: OffsetDateTime? = null) = EmisPrescriptionsBuilder(
            configuration!!,
            patient.endUserSessionId,
            patient.sessionId,
            patient.userPatientLinkToken,
            fromDate,
            toDate)

    fun coursesRequest(patient: Patient) = EmisCoursesBuilder(
            configuration!!,
            patient.endUserSessionId,
            patient.sessionId,
            patient.userPatientLinkToken)

    fun repeatPrescriptionSubmissionRequest(patient: Patient,
                                            prescriptionSubmissionRequest: PrescriptionSubmissionRequest? = null) =
            EmisPrescriptionsSubmissionBuilder(
                    configuration!!,
                    patient.endUserSessionId,
                    patient.sessionId,
                    patient.userPatientLinkToken,
                    prescriptionSubmissionRequest)
}