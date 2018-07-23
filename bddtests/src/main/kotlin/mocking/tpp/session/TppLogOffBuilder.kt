package mocking.tpp.session

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import mocking.tpp.models.LogOffReply
import org.apache.http.HttpStatus
import javax.xml.bind.JAXBContext
import java.io.StringWriter
import javax.xml.bind.Marshaller


class TppLogOffBuilder() : TppMappingBuilder("POST", "/tpp/") {
    init {
        val typeHeader = "type"
        val typeValue = "Logoff"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//Logoff[" +
                        "@apiVersion='${apiVersion}' and " +
                        "@uuid='${uuid}']")
    }

    fun respondWithSuccess(): Mapping {
        val responseBody = LogOffReply(uuid)
        return respondWith(responseBody)
    }

    fun respondWithError(): Mapping {
        var error = Error("5", "You must be logged on" , uuid)
        return respondWith(error)
    }
}