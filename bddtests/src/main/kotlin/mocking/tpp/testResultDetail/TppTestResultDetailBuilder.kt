package mocking.tpp.testResultDetail

import mocking.GsonFactory
import mocking.emis.models.ExceptionResponse
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.TestResultsViewReply
import worker.models.demographics.TppUserSession
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller
import mocking.models.Mapping
import org.apache.http.HttpStatus
import java.time.OffsetDateTime

class TppTestResultDetailBuilder(tppUserSession: TppUserSession, testResultId: String) : TppMappingBuilder("POST", "/tpp/") {
    init {
        val typeHeader = "type"
        val typeValue = "TestResultsView"
        val apiVersion = "1"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//TestResultsView[" +
                        "@apiVersion='${apiVersion}' and " +
                        "@patientId='${tppUserSession.patientId}' and " +
                        "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                        "@unitId='${tppUserSession.unitId}' and " +
                        "@testResultId='$testResultId'")
    }

    fun respondWithSuccess(testResultsViewReply: TestResultsViewReply): Mapping {
        val suidHeader = "suid"
        val suidValue = "alsdkfjLIKASDLIHUAJakjshdLIASKHDJALsdiojALSasIADJAISDioasjd"

        val jaxbContext = JAXBContext.newInstance(TestResultsViewReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(testResultsViewReply, stringWriter)
        }

        var resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .andHeader(suidHeader, suidValue)
                    .build()
        }

        return resp
    }

    fun respondWithServiceNotAvailableException(): Mapping {
        val exceptionResponse = ExceptionResponse(503,
                "Service unavailable")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}