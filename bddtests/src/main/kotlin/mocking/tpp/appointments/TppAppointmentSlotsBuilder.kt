package mocking.tpp.appointments

import mocking.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
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
import java.time.OffsetDateTime

class TppAppointmentSlotsBuilder(tppUserSession: TppUserSession) :
        TppMappingBuilder("POST", "/tpp/"), IAppointmentSlotsBuilder {

    val tppUserSession: TppUserSession = tppUserSession

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val typeHeader = "type"
        val typeValue = "ListSlots"
        val apiVersion = "1"

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//ListSlots[" +
                        "@apiVersion='$apiVersion' and " +
                        "@patientId='${tppUserSession.patientId}' and " +
                        "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                        "@unitId='${tppUserSession.unitId}']")
    }

    override fun respondWithSuccess(slots: ArrayList<AppointmentSlotFacade>, sessionId: Int?, sessionDate: String?): Mapping {
        TODO("not implemented")
    }

    private fun respondWithSuccess(listSlotsReply: ListSlotsReply): Mapping {

        val xmlBody = serialsier(listSlotsReply)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody)
                    .build()
        }
    }

    override fun withDelay(delayMilliseconds: Duration): IAppointmentSlotsBuilder {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithSuccess(model: AppointmentSlotsResponseFacade): Mapping {
        return respondWithSuccess(listSlotsReplyConverter(model))
    }

    private fun listSlotsReplyConverter(model: AppointmentSlotsResponseFacade): ListSlotsReply {
        return ListSlotsReply(patientId = tppUserSession.patientId,
                onlineUserId = tppUserSession.onlineUserId,
                uuid = uuid,
                bookableDays = model.bookableDays!!,
                Session = sessionsConverter(model.sessions))
    }

    private fun sessionsConverter(sessions: ArrayList<AppointmentSessionFacade>): MutableCollection<Session> {

        val sessionsList: MutableCollection<Session> = mutableListOf()
        sessions.forEach { session -> sessionsList.add(sessionConverter(session)) }
        return sessionsList
    }

    private fun sessionConverter(session: AppointmentSessionFacade): Session {
        val slots: MutableCollection<Slot> = mutableListOf()
        session.slots.forEach { slot -> slots.add(slotConverter(slot)) }
        return Session(
                sessionId = getValueOrTestSetupIncorrectly(session.sessionId, "sessionId"),
                type = getValueOrTestSetupIncorrectly(session.sessionType, "sessionType"),
                staffDetails = getValueOrTestSetupIncorrectly(session.staffDetails, "staffDetails"),
                location = getValueOrTestSetupIncorrectly(session.location!!, "location"),
                Slot = slots
        )
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


    private fun slotConverter(slot: AppointmentSlotFacade): Slot {
        return Slot(
                startDate = "${slot.startTime!!}.0Z",
                endDate = "${slot.endTime!!}.0Z",
                type = slot.slotTypeName!!)
    }

    override fun respondWithSuccess(body: String): Mapping {
        TODO("not implemented")
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        TODO("not implemented")
    }

    override fun respondWithUnknownException(): Mapping {
        TODO("not implemented")
    }

    private fun getDateFormattedString(dateTime: OffsetDateTime): String {
        return String.format("%s-%s-%s", dateTime.year, formatDateToTwoDigits(dateTime.monthValue), formatDateToTwoDigits(dateTime.dayOfMonth))
    }

    private fun formatDateToTwoDigits(daysOrMonths: Int): String {
        return String.format("%02d", daysOrMonths)
    }
}
