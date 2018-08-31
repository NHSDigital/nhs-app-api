package mocking.emis

import constants.EmisResponseCode
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.MappingBuilder
import mocking.emis.courses.EmisCoursesBuilder
import mocking.emis.demographics.EmisDemographicsBuilder
import mocking.emis.allergies.EmisAllergiesBuilder
import mocking.emis.appointments.AppointmentSlotsBuilderEmis
import mocking.emis.appointments.AppointmentSlotsMetaBuilderEmis
import mocking.emis.appointments.BookAppointmentsBuilderEmis
import mocking.emis.appointments.GetAppointmentBuilderEmis
import mocking.emis.appointments.DeleteAppointmentsBuilderEmis
import mocking.emis.immunisations.EmisImmunisationsBuilder
import mocking.emis.me.EmisMeApplicationsBuilder
import mocking.emis.me.EmisMeBuilder
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.medications.EmisMedicationsBuilder
import mocking.emis.prescriptions.EmisPrescriptionsBuilder
import mocking.emis.prescriptionsSubmission.EmisPrescriptionsSubmissionBuilder
import mocking.emis.session.EmisEndUserSessionBuilder
import mocking.emis.session.EmisSessionBuilder
import mocking.emis.testResults.EmisTestResultsBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus
import mocking.emis.consultations.EmisConsultationsBuilder
import mocking.emis.problems.EmisProblemsBuilder
import mocking.emis.linkage.EmisLinkageGETBuilder
import mocking.emis.linkage.EmisLinkagePOSTBuilder
import mocking.emis.practices.PracticeSettingsBuilderEmis
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddVerificationRequest
import mocking.emis.models.BadRequestResponse
import mocking.emis.models.ErrorResponse
import mocking.emis.models.ExceptionResponse
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.OffsetDateTime

const val HEADER_API_APPLICATION_ID = "X-API-ApplicationId"
const val HEADER_API_END_USER_SESSION_ID = "X-API-EndUserSessionId"
const val HEADER_API_SESSION_ID = "X-API-SessionId"
const val HEADER_API_VERSION = "X-API-Version"
const val HEADER_NHS_NUMBER = "nhsNumber"
const val HEADER_ODS_CODE = "odsCode"
const val QUERY_PARAM_USER_PATIENT_LINK_TOKEN = "userPatientLinkToken"

@Suppress("TooManyFunctions")
open class EmisMappingBuilder(private var configuration: EmisConfiguration?,
                              private val method: String, relativePath: String)
    : MappingBuilder(method, "/emis$relativePath"), IAppointmentMappingBuilder {

    protected var delayMillisecs = 0

    init {
        if (configuration != null) {
            requestBuilder
                    .andHeader(HEADER_API_APPLICATION_ID, configuration!!.applicationId)
                    .andHeader(HEADER_API_VERSION, configuration!!.version)
        }
    }

    fun practiceSettingsRequest(patient: Patient) = PracticeSettingsBuilderEmis(patient)

    override fun viewMyAppointmentsRequest(patient: Patient):
            IMyAppointmentsBuilder = GetAppointmentBuilderEmis(configuration, patient)

    override fun appointmentSlotsRequest(patient: Patient,
                                         fromDateTime: String?, toDateTime: String?) = AppointmentSlotsBuilderEmis(
            configuration!!,
            patient.endUserSessionId,
            patient.sessionId,
            fromDateTime,
            toDateTime,
            patient.userPatientLinkToken)

    fun appointmentSlotsMetaRequest(patient: Patient,
                                    sessionStartDate: String? = null,
                                    sessionEndDate: String? = null) = AppointmentSlotsMetaBuilderEmis(
            configuration!!,
            patient.endUserSessionId,
            patient.sessionId,
            sessionStartDate,
            sessionEndDate,
            patient.userPatientLinkToken)

    override fun bookAppointmentSlotRequest(patient: Patient,
                                            request: BookAppointmentSlotFacade) =
            BookAppointmentsBuilderEmis(configuration!!, patient.endUserSessionId, patient.sessionId, request)

    override fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade)
            = DeleteAppointmentsBuilderEmis(configuration!!, patient, request)

    fun demographicsRequest(patient: Patient) = EmisDemographicsBuilder(configuration!!,
                                                                        patient.userPatientLinkToken,
                                                                        patient.endUserSessionId, patient.sessionId)

    fun allergiesRequest(patient: Patient) = EmisAllergiesBuilder(configuration!!,
                                                                  patient.userPatientLinkToken,
                                                                  patient.endUserSessionId,
                                                                  patient.sessionId)

    fun medicationsRequest(patient: Patient) = EmisMedicationsBuilder(configuration!!,
                                                                      patient.userPatientLinkToken,
                                                                      patient.endUserSessionId,
                                                                      patient.sessionId)

    fun problemsRequest(patient: Patient) = EmisProblemsBuilder(configuration!!,
                                                                patient.userPatientLinkToken,
                                                                patient.endUserSessionId,
                                                                patient.sessionId)

    fun consultationsRequest(patient: Patient) = EmisConsultationsBuilder(configuration!!,
                                                                          patient.userPatientLinkToken,
                                                                          patient.endUserSessionId,
                                                                          patient.sessionId)

    fun immunisationsRequest(patient: Patient) = EmisImmunisationsBuilder(configuration!!,
                                                                          patient.userPatientLinkToken,
                                                                          patient.endUserSessionId,
                                                                          patient.sessionId)

    fun testResultsRequest(patient: Patient) = EmisTestResultsBuilder(configuration!!,
                                                                      patient.userPatientLinkToken,
                                                                      patient.endUserSessionId,
                                                                      patient.sessionId)

    fun meRequest(patient: Patient) = EmisMeBuilder(configuration!!, method, patient)

    fun meApplicationsRequest(patient: Patient, model: LinkApplicationRequestModel) = EmisMeApplicationsBuilder(
            configuration!!, patient.endUserSessionId, model)

    fun endUserSessionRequest() = EmisEndUserSessionBuilder(configuration!!)

    fun sessionRequest(patient: Patient) = EmisSessionBuilder(configuration!!, patient)

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

    fun respondWithBadRequest(message: String, fieldName: String): Mapping {
        val responseBody = BadRequestResponse(message, fieldName)
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun linkageKeyGetRequest(request: AddVerificationRequest) = EmisLinkageGETBuilder(request)

    fun linkageKeyPOSTRequest(request: AddNhsUserRequest) = EmisLinkagePOSTBuilder(request)

    fun responseErrorForbiddenService(): Mapping {
        return respondWithStandardError(EmisResponseCode.SERVICE_ACCESS_VIOLATION.toInt(), HttpStatus.SC_FORBIDDEN)
    }

    fun respondWithStandardError(internalResponseCode: Int, httpResponseCode: Int): Mapping {
        val responseBody = ErrorResponse(internalResponseCode)
        return respondWith(httpResponseCode) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    protected fun respondWithException(internalResponseCode: Int, message: String): Mapping {

        val responseBody = ExceptionResponse(internalResponseCode.toLong(), message)

        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(responseBody)
                    .build()
        }
    }
}
