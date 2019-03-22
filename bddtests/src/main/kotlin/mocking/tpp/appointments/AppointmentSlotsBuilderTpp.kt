package mocking.tpp.appointments

import constants.DateTimeFormats
import constants.ErrorResponseCodeTpp
import mocking.JSonXmlConverter
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.ListSlotsReply
import mocking.tpp.models.Session
import mocking.tpp.models.Slot
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus
import org.junit.Assert.fail
import worker.models.demographics.TppUserSession
import java.time.Duration
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.time.temporal.ChronoUnit
import java.util.*

class AppointmentSlotsBuilderTpp(
        val tppUserSession: TppUserSession,
        startDate: ZonedDateTime?,
        endDate: ZonedDateTime?
) : TppMappingBuilder("POST", "/tpp/"), IAppointmentSlotsBuilder {

    private val appointmentSlotsExample = AppointmentsSlotsExample()

    init {
        val typeHeader = "type"
        val typeValue = "ListSlots"
        val apiVersion = "1"

        val path = StringBuilder("//ListSlots[")

        if (startDate != null) {
            //dropLast is dropping the separator and last digit of minutes below to ensure mocks
            //are valid for more than 1 min
            val startDateParts = convertDateToTppTimeString(startDate).split(":")
            path.append("starts-with(@startDate, ")
            path.append("'${startDateParts.joinToString(":", limit = 2, truncated = "").dropLast(2)}') and ")
            path.append("substring(@startDate, string-length(@startDate)) = 'Z' and ")
        }

        if (endDate != null) {
            val numberOfDays = getNumberOfDays(startDate, endDate)
            path.append("@numberOfDays='$numberOfDays' and ")
        }


        path.append("@apiVersion='$apiVersion' and " +
                "@patientId='${tppUserSession.patientId}' and " +
                "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                "@unitId='${tppUserSession.unitId}']")

        requestBuilder.andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath(path.toString())
    }

    private fun getNumberOfDays(startDate: ZonedDateTime?, endDate: ZonedDateTime?): Long {
        // "between" excludes the last date
        return ChronoUnit.DAYS.between(startDate, endDate!!.plusDays(1))
    }

    override fun withDelay(delayMilliseconds: Duration): IAppointmentSlotsBuilder {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        val errorMsg = "You don't have access to this online service"
        val disabledTppError = Error(
                errorCode = ErrorResponseCodeTpp.NO_ACCESS,
                userFriendlyMessage = errorMsg,
                uuid = UUID.randomUUID().toString())
        return respondWith(disabledTppError)
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithTppUnknownError("Unknown exception")
    }

    override fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping {
        return respondWithSuccess(listSlotsReplyConverter(facade))
    }

    private fun respondWithSuccess(listSlotsReply: ListSlotsReply): Mapping {

        val xmlBody = JSonXmlConverter.toXML(listSlotsReply)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody).andDelay(delayMillisecs)
                    .build()
        }
    }

    override fun respondWithCorrupted(): Mapping {
        val model = appointmentSlotsExample.multipleSlotsOneLocation()
        val response = JSonXmlConverter.toXML(listSlotsReplyConverter(model))
        return respondWithCorruptedContent(response)
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
    }

    private fun listSlotsReplyConverter(model: AppointmentSlotsResponseFacade): ListSlotsReply {

        val sessionsList: MutableCollection<Session> = mutableListOf()
        model.sessions.forEach { session ->
            sessionsList.addAll(
                    session.slots.map { slot -> slotConverter(model, session, slot) })
        }

        return ListSlotsReply(patientId = tppUserSession.patientId,
                onlineUserId = tppUserSession.onlineUserId,
                uuid = TppConfig.uuid,
                bookableDays = model.bookableDays,
                Session = sessionsList)
    }

    private fun slotConverter(
            fullResponse: AppointmentSlotsResponseFacade,
            session: AppointmentSessionFacade,
            slot:
            AppointmentSlotFacade
    ): Session {

        return Session(
                sessionId = getValueOrTestSetupIncorrectly(slot.slotId, "sessionId"),
                type = getValueOrTestSetupIncorrectly(session.sessionType, "sessionType"),
                staffDetails = getValueOrTestSetupIncorrectly(session.staffDetails.map { clinician ->
                    fullResponse.staffDetails.find { staff ->
                        clinician == staff.staffDetailsid
                    }!!.staffName
                }.first(), "staffName"),
                location = getValueOrTestSetupIncorrectly(fullResponse.locations.find { location ->
                    session.locationId == location.locationId
                }!!.locationName, "location"),
                Slot = mutableListOf(Slot(
                        startDate = convertStringToTppTimeString(slot.startTime!!),
                        endDate = convertStringToTppTimeString(slot.endTime!!),
                        type = fullResponse.slotTypes.find {
                            slotType -> slot.slotTypeId == slotType.slotTypeId
                        }!!.slotTypeName
                )))
    }

    private fun convertStringToTppTimeString(time: String): String {
        return convertDateToTppTimeString(ZonedDateTime.parse(time))
    }

    private fun convertDateToTppTimeString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.tppDateTimeFormat)
        return queryDateFormat.format(time)
    }

    private fun getValueOrTestSetupIncorrectly(value: Int?, valueName: String): String {
        if (value == null) {
            fail("Test setup incorrectly, $valueName must be set")
        }
        return value!!.toString()
    }

    private fun getValueOrTestSetupIncorrectly(value: String?, valueName: String): String {
        if (value == null) {
            fail("Test setup incorrectly, $valueName must be set")
        }
        return value!!
    }
}
