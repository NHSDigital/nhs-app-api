package mocking.emis

import mocking.MappingBuilder
import mocking.emis.appointments.EmisAppointmentSlotsBuilder
import mocking.emis.appointments.EmisAppointmentSlotsMetaBuilder
import mocking.emis.courses.EmisCoursesBuilder
import mocking.emis.demographics.EmisDemographicsBuilder
import mocking.emis.me.EmisMeBuilder
import mocking.emis.models.ExceptionResponse
import mocking.emis.prescriptions.EmisPrescriptionsBuilder
import mocking.emis.session.EmisEndUserSessionBuilder
import mocking.emis.session.EmisSessionBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus
import java.time.OffsetDateTime

const val HEADER_API_APPLICATION_ID = "X-API-ApplicationId"
const val HEADER_API_END_USER_SESSION_ID = "X-API-EndUserSessionId"
const val HEADER_API_SESSION_ID = "X-API-SessionId"
const val HEADER_API_VERSION = "X-API-Version"
const val QUERY_PARAM_USER_PATIENT_LINK_TOKEN = "userPatientLinkToken"

open class EmisMappingBuilder(private val configuration: EmisConfiguration, private val method: String, relativePath: String) : MappingBuilder(method, "/emis$relativePath") {
    init {
        requestBuilder
                .andHeader(HEADER_API_APPLICATION_ID, configuration.applicationId)
                .andHeader(HEADER_API_VERSION, configuration.version)
    }

    fun appointmentSlotsRequest(patient: Patient, fromDateTime: String? = null, toDateTime: String? = null, sessionId: String? = null) = EmisAppointmentSlotsBuilder(
            configuration,
            patient.endUserSessionId,
            patient.sessionId,
            fromDateTime,
            toDateTime,
            sessionId)

    fun appointmentSlotsMetaRequest(patient: Patient, sessionStartDate: String? = null, sessionEndDate: String? = null) = EmisAppointmentSlotsMetaBuilder(
            configuration,
            patient.endUserSessionId,
            patient.userPatientLinkToken,
            sessionStartDate,
            sessionEndDate,
            patient.userPatientLinkToken)

    fun demographicsRequest(patient: Patient) = EmisDemographicsBuilder(configuration, patient.userPatientLinkToken, patient.endUserSessionId, patient.sessionId)

    fun meRequest(patient: Patient) = EmisMeBuilder(configuration, method, patient)

    fun endUserSessionRequest() = EmisEndUserSessionBuilder(configuration)

    fun sessionRequest(patient: Patient) = EmisSessionBuilder(configuration, patient)

    fun prescriptionsRequest(patient: Patient, fromDate: OffsetDateTime, toDate: OffsetDateTime) = EmisPrescriptionsBuilder(
            configuration,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId,
            fromDate,
            toDate)

    fun coursesRequest(patient: Patient) = EmisCoursesBuilder(
            configuration,
            patient.endUserSessionId,
            patient.userPatientLinkToken,
            patient.sessionId)

    protected fun respondWithException(internalResponseCode: Int, message: String): Mapping {

        val responseBody = ExceptionResponse(internalResponseCode.toLong(), message)

        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(responseBody)
                    .build()
        }
    }

}