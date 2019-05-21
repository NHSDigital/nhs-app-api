package mocking.tpp.messages

import mocking.JSonXmlConverter
import mocking.defaults.TppMockDefaults
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.RequestSystmOnlineMessagesReply
import models.Patient
import org.apache.http.HttpStatus

class MessagesBuilderTpp(
        val patient: Patient
) : TppMappingBuilder("POST", "/tpp/") {

    init {
        val path = StringBuilder("//RequestSystmOnlineMessages[" +
                "@patientId='${patient.patientId}'" +
                " and " +
                "@onlineUserId='${patient.onlineUserId}'" +
                "]")

        requestBuilder.andBodyMatchingXpath(path.toString())
    }

    fun respondWithSuccess(message: String?): Mapping {
        val responseBody = RequestSystmOnlineMessagesReply(
                patientId = patient.patientId,
                onlineUserId = patient.onlineUserId,
                uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                bookAppointments = message
        )
        val xmlBody = JSonXmlConverter.toXML(responseBody)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody).build()
        }
    }

    fun respondWithError(errorBody: Error): Mapping {
        val responseBody = Error(
                errorBody.errorCode,
                errorBody.userFriendlyMessage,
                errorBody.uuid
        )

        val xmlBody = JSonXmlConverter.toXML(responseBody)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody).build()
        }
    }
}
