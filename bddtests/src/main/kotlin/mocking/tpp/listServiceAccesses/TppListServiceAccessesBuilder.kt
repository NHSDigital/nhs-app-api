package mocking.tpp.listServiceAccesses

import mocking.defaults.TppMockDefaults.Companion.DEFAULT_TPP_SESSION_ID
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.ListServiceAccessesReply
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppListServiceAccessesBuilder(tppUserSession: TppUserSession): TppMappingBuilder() {
    init {
        val typeHeader = "type"
        val typeValue = "ListServiceAccesses"
        val apiVersion = "1"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//ListServiceAccesses[" +
                                              "@apiVersion='${apiVersion}' and " +
                                              "@patientId='${tppUserSession.patientId}' and " +
                                              "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                                              "@unitId='${tppUserSession.unitId}']")
    }

    fun respondWithSuccess(listServiceAccessesReply: ListServiceAccessesReply): Mapping {
        val suidHeader = "suid"
        val suidValue = DEFAULT_TPP_SESSION_ID

        val jaxbContext = JAXBContext.newInstance(ListServiceAccessesReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(listServiceAccessesReply, stringWriter)
        }

        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .andHeader(suidHeader, suidValue)
                    .build()
        }

        return resp
    }
}