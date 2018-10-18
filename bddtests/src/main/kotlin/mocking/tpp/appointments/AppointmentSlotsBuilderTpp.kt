package mocking.tpp.appointments

import constants.DateTimeFormats
import mocking.JSonXmlConverter
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
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.time.temporal.ChronoUnit
import java.util.*

class AppointmentSlotsBuilderTpp(
        val tppUserSession: TppUserSession,
        startDate: String? = null,
        endDate: String? = null
) : TppMappingBuilder("POST", "/tpp/"), IAppointmentSlotsBuilder {

    init {
        val typeHeader = "type"
        val typeValue = "ListSlots"
        val apiVersion = "1"

        val path = StringBuilder("//ListSlots[")

        if (startDate != null) {
            path.append("@startDate='$startDate' and ")
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

    private fun getNumberOfDays(startDate: String, endDate: String? = null): Long {

        if (endDate != null) {
            val format = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
            val firstDate = LocalDate.parse(startDate, format)
            val secondDate = LocalDate.parse(endDate, format)

            return ChronoUnit.DAYS.between(firstDate, secondDate)
        }
        return 1
    }

    override fun withDelay(delayMilliseconds: Duration): IAppointmentSlotsBuilder {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        val errorMsg = "You don't have access to this online service"
        val disabledTppError = Error(
                errorCode = "6",
                userFriendlyMessage = errorMsg,
                uuid = UUID.randomUUID().toString())
        return respondWith(disabledTppError)
    }

    override fun respondWithUnknownException(): Mapping {
        throw NotImplementedError("Not Implemented. ")
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

    private fun listSlotsReplyConverter(model: AppointmentSlotsResponseFacade): ListSlotsReply {

        val sessionsList: MutableCollection<Session> = mutableListOf()
        model.sessions.forEach { session ->
            sessionsList.addAll(
                    session.slots.map { slot -> slotConverter(session, slot) })
        }

        return ListSlotsReply(patientId = tppUserSession.patientId,
                onlineUserId = tppUserSession.onlineUserId,
                uuid = TppConfig.uuid,
                bookableDays = model.bookableDays!!,
                Session = sessionsList)
    }

    private fun slotConverter(session: AppointmentSessionFacade, slot: AppointmentSlotFacade): Session {

        return Session(
                sessionId = getValueOrTestSetupIncorrectly(slot.slotId, "sessionId"),
                type = getValueOrTestSetupIncorrectly(session.sessionType, "sessionType"),
                staffDetails = getValueOrTestSetupIncorrectly(session.staffDetails.first().staffName, "staffName"),
                location = getValueOrTestSetupIncorrectly(session.location, "location"),
                Slot = mutableListOf(Slot(
                        startDate = convertDateToTppTime(slot.startTime!!),
                        endDate = convertDateToTppTime(slot.endTime!!),
                        type = slot.slotTypeName!!)))
    }

    private fun convertDateToTppTime(time: String): String {
        val currentDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
        val dateToPass = ZonedDateTime.of(LocalDateTime.parse(time, currentDateFormat), ZoneId.of
        ("Europe/London"))
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.tppDateTimeFormat)
        return queryDateFormat.format(dateToPass)
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
