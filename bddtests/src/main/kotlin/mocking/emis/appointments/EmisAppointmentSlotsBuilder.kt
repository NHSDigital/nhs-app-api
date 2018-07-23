package mocking.emis.appointments

import mocking.GsonFactory
import mocking.IAppointmentSlotsBuilder
import mocking.defaults.MockDefaults
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.AppointmentSession
import mocking.emis.models.AppointmentSlot
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import java.time.Duration

class EmisAppointmentSlotsBuilder(configuration: EmisConfiguration,
                                  apiEndUserSessionId: String,
                                  apiSessionId: String,
                                  fromDateTime: String?,
                                  toDateTime: String?,
                                  linkToken: String?)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots"), IAppointmentSlotsBuilder {

    init {
        if (apiEndUserSessionId.isEmpty()) requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, MockDefaults.patient.endUserSessionId)
        else requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
        if (apiSessionId.isEmpty()) requestBuilder.andHeader(HEADER_API_SESSION_ID, MockDefaults.patient.sessionId)
        else requestBuilder.andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (!fromDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "fromDateTime", value = fromDateTime!!)
        if (!toDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "toDateTime", value = toDateTime!!)
        if (!linkToken.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "userPatientLinkToken", value = linkToken!!)
    }

    override fun respondWithSuccess(slots: ArrayList<AppointmentSlotFacade>, sessionId: Int?, sessionDate: String?): Mapping {
        var emisSlots = slotsConverter(slots)
        val session = AppointmentSession(sessionDate, sessionId, emisSlots)
        val model = GetAppointmentSlotsResponseModel(arrayListOf(session))
        return respondWithBody(model)
    }

    private fun slotsConverter(slots: ArrayList<AppointmentSlotFacade>) : ArrayList<AppointmentSlot>{
        var list : ArrayList<AppointmentSlot> =arrayListOf()
        slots.forEach{slot->  list.add(slotConverter(slot))}
        return list
    }

    private fun slotConverter(slot : AppointmentSlotFacade): AppointmentSlot{

        return AppointmentSlot(slotId = slot.slotId!!,
                startTime = slot.startTime,
                endTime = slot.endTime,
                slotTypeName = slot.slotTypeName
        )
    }

    private var delayMillisecs = 0

    override fun withDelay(delayMilliseconds : Duration):EmisAppointmentSlotsBuilder{
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(model: AppointmentSlotsResponseFacade): Mapping {
        return respondWithBody(model)
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(body, contentType = "application/json")
        }
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(-1030,
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'AppointmentBooking' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. Available services are 'AddressChange, RecordViewer, RepeatPrescribing, SharedRecordAuditView'. Extra info: Services Access violation")
        return respondWithException(exceptionResponse)
    }

    override fun respondWithUnknownException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
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