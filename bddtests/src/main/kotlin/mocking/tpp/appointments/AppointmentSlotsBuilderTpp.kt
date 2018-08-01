package mocking.tpp.appointments

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.JSonXmlConverter
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.data.TppConfig
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
import java.time.OffsetDateTime
import java.util.*

class AppointmentSlotsBuilderTpp(tppUserSession: TppUserSession) :
        TppMappingBuilder("POST", "/tpp/"), IAppointmentSlotsBuilder {

    val tppUserSession: TppUserSession = tppUserSession

    init {
        val typeHeader = "type"
        val typeValue = "ListSlots"
        val apiVersion = "1"

        requestBuilder.andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//ListSlots[" +
                        "@apiVersion='$apiVersion' and " +
                        "@patientId='${tppUserSession.patientId}' and " +
                        "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                        "@unitId='${tppUserSession.unitId}']")
    }

    override fun withDelay(delayMilliseconds: Duration): IAppointmentSlotsBuilder {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        val errorMsg = "You don't have access to this online service"
        val disabledTppError = Error(errorCode = "6", userFriendlyMessage = errorMsg, uuid = UUID.randomUUID().toString())
        return respondWith(disabledTppError)
    }

    override fun respondWithUnknownException(): Mapping {
        TODO("not implemented")
    }

    override fun respondWithSuccess(model: AppointmentSlotsResponseFacade): Mapping {
        return respondWithSuccess(listSlotsReplyConverter(model))
    }

    private fun respondWithSuccess(listSlotsReply: ListSlotsReply): Mapping {

        val xmlBody = JSonXmlConverter.toXML(listSlotsReply)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody).andDelay(delayMillisecs)
                    .build()
        }
    }

    private fun listSlotsReplyConverter(model: AppointmentSlotsResponseFacade): ListSlotsReply {
        return ListSlotsReply(patientId = tppUserSession.patientId,
                onlineUserId = tppUserSession.onlineUserId,
                uuid = TppConfig.uuid,
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
}
