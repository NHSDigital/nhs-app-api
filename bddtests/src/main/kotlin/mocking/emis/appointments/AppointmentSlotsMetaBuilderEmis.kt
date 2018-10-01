package mocking.emis.appointments

import constants.EmisResponseCode
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.appointments.helpers.AppointmentSlotsMetaHelper
import mocking.emis.models.ExceptionResponse
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import java.time.Duration

class AppointmentSlotsMetaBuilderEmis(
        configuration: EmisConfiguration,
        apiEndUserSessionId: String,
        apiSessionId: String,
        sessionStartDate: String? = null,
        sessionEndDate: String? = null,
        userPatientLinkToken: String? = null)
    : EmisMappingBuilder(configuration, method = "GET",
        relativePath = "/appointmentslots/meta"), IAppointmentSlotsBuilder {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        requestBuilder.andQueryParameterIfNotNull("sessionStartDate", sessionStartDate)
        requestBuilder.andQueryParameterIfNotNull("sessionEndDate", sessionEndDate)
        requestBuilder.andQueryParameterIfNotNull("userPatientLinkToken", userPatientLinkToken)
    }

    override fun withDelay(delayMilliseconds: Duration): AppointmentSlotsMetaBuilderEmis {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping {
        val locations = AppointmentSlotsMetaHelper.getMetaSlotLocationsList(facade.sessions)
        val sessionHolders = AppointmentSlotsMetaHelper.getMetaSlotSessionHoldersList(facade.sessions)
        val slotSessions = AppointmentSlotsMetaHelper.getMetaSlotSessionsList(facade.sessions)

        val appointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
                locations,
                sessionHolders,
                slotSessions
        )
        return respondWithSuccess(appointmentSlotsMetaResponseModel)
    }

    fun respondWithSuccess(model: GetAppointmentSlotsMetaResponseModel): Mapping {
        return respondWithBody(model)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(EmisResponseCode.SERVICE_ACCESS_VIOLATION,
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services " +
                        "'AppointmentBooking' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked " +
                        "patient. Available services are 'AddressChange, RecordViewer, RepeatPrescribing, " +
                        "SharedRecordAuditView'. Extra info: Services Access violation")
        return respondWithException(exceptionResponse)
    }

    override fun respondWithUnknownException(): Mapping {
        val exceptionResponse = ExceptionResponse(EmisResponseCode.EXCEPTION,
                "Unknown Exception")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = SC_OK): Mapping {

        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
                    .andDelay(delayMillisecs)

        }
    }
}
