package mocking.emis.appointments

import constants.EmisResponseCode
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mocking.emis.models.ExceptionResponse
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import java.time.Duration

private const val DEFAULT_DURATION: Int = 10
private const val NUMBER_OF_SLOTS: Int = 1

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
    
    override fun withDelay(delayMilliseconds : Duration):AppointmentSlotsMetaBuilderEmis{
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(model: AppointmentSlotsResponseFacade): Mapping {
        val locations = getMetaSlotLocationsList(model.sessions)
        val sessionHolders = getMetaSlotSessionHoldersList(model.sessions)
        val slotSessions = getMetaSlotSessionsList(model.sessions)

        val appointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
                locations,
                sessionHolders,
                slotSessions
        )
        return respondWithSuccess(appointmentSlotsMetaResponseModel)
    }


    private fun getMetaSlotLocationsList(sessions: ArrayList<AppointmentSessionFacade>): ArrayList<Location> {
        val arrayList = arrayListOf<Location>()
        arrayList.addAll( sessions.map {  session-> Location(session.locationid!!, session.location!!) })
        return arrayList
    }

    private fun getMetaSlotSessionHoldersList(sessions: ArrayList<AppointmentSessionFacade>): ArrayList<SessionHolder> {
        val arrayList = arrayListOf<SessionHolder>()
        arrayList.addAll( sessions.map {  session-> SessionHolder(session.staffDetailsid!!, session.staffDetails!!) })
        return arrayList
    }

    private fun getMetaSlotSessionsList(sessions: ArrayList<AppointmentSessionFacade>):ArrayList<Session> {
        val arrayList = arrayListOf<Session>()
        arrayList.addAll(sessions.flatMap { session ->
            session.slots.map { slot ->
                Session(session.sessionType!!,
                        slot.slotId!!,
                        session.locationid,
                        DEFAULT_DURATION,
                        SessionType.Timed,
                        NUMBER_OF_SLOTS,
                        arrayListOf(session.staffDetailsid!!),
                        slot.startTime,
                        slot.endTime)
            }
        })
        return arrayList
    }

    fun respondWithSuccess(model: GetAppointmentSlotsMetaResponseModel): Mapping {
        return respondWithBody(model)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(EmisResponseCode.SERVICE_ACCESS_VIOLATION,
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'AppointmentBooking' " +
                "from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. " +
                "Available services are 'AddressChange, RecordViewer, RepeatPrescribing, SharedRecordAuditView'. " +
                "Extra info: Services Access violation")
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
