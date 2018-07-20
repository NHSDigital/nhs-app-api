package mocking.tpp.viewPatientOverview

import mocking.GsonFactory
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.ViewPatientOverviewReply
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import javax.xml.bind.JAXBContext
import java.io.StringWriter
import javax.xml.bind.Marshaller


class TppViewPatientOverviewBuilder(tppUserSession: TppUserSession) : TppMappingBuilder("POST", "/tpp/") {
    init {
        val typeHeader = "type"
        val typeValue = "ViewPatientOverview"
        val apiVersion = "1"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//ViewPatientOverview[" +
                        "@apiVersion='${apiVersion}' and " +
                        "@patientId='${tppUserSession.patientId}' and " +
                        "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                        "@unitId='${tppUserSession.unitId}']")
    }

    fun respondWithSuccess(viewPatientOverviewReply: ViewPatientOverviewReply): Mapping {
        val responseBody = ViewPatientOverviewReply(
                drugs = viewPatientOverviewReply.drugs,
                drugSensitivities = viewPatientOverviewReply.drugSensitivities,
                currentRepeats = viewPatientOverviewReply.currentRepeats,
                pastRepeats = viewPatientOverviewReply.pastRepeats,
                allergies = viewPatientOverviewReply.allergies
        )

        val suidHeader = "suid"
        val suidValue = "alsdkfjLIKASDLIHUAJakjshdLIASKHDJALsdiojALSasIADJAISDioasjd"

        val jaxbContext = JAXBContext.newInstance(ViewPatientOverviewReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(responseBody, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .andHeader(suidHeader, suidValue)
                    .build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(500,
                "Requested record access is disabled by the practice")
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

    fun respondWithError(errorBody: Error): Mapping {
        val responseBody = Error(
                errorBody.errorCode,
                errorBody.userFriendlyMessage,
                errorBody.uuid
        )

        val jaxbContext = JAXBContext.newInstance(Error::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(responseBody, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .build()
        }
    }
}