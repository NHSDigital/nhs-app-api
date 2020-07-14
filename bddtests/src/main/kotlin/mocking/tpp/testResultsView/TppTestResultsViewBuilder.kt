package mocking.tpp.testResultsView

import mocking.GsonFactory
import mocking.defaults.TppMockDefaults.Companion.DEFAULT_TPP_SESSION_ID
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.TestResultsViewReply
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import java.io.StringWriter
import java.time.OffsetDateTime
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppTestResultsViewBuilder(tppUserSession: TppUserSession, startDate: OffsetDateTime,
                                endDate: OffsetDateTime) : TppMappingBuilder("POST", "/tpp/") {
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
                        "starts-with(@startDate, '${getDateFormattedString(startDate)}') and " +
                        "starts-with(@endDate, '${getDateFormattedString(endDate)}')]")

    }

    fun respondWithSuccess(testResultsViewReply: TestResultsViewReply): Mapping {
        val suidHeader = "suid"
        val suidValue = DEFAULT_TPP_SESSION_ID

        val jaxbContext = JAXBContext.newInstance(TestResultsViewReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use { marshaller.marshal(testResultsViewReply, stringWriter)
        }

        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString()).andHeader(suidHeader, suidValue).build()
                }
        return resp
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
        stringWriter.use { marshaller.marshal(responseBody, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) { andXmlBody(stringWriter.toString()).build()
        }
    }

    fun respondWithServiceNotAvailableException(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_SERVICE_UNAVAILABLE.toLong(),
                        "Service unavailable")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) { andJsonBody(body, GsonFactory.asPascal)
        }
    }

    private fun getDateFormattedString(dateTime: OffsetDateTime): String {
        return String.format(
                "%s-%s-%s",
                dateTime.year,
                formatDateToTwoDigits(dateTime.monthValue),
                formatDateToTwoDigits(dateTime.dayOfMonth))
    }

    private fun formatDateToTwoDigits(daysOrMonths: Int): String {
        return String.format("%02d", daysOrMonths)
    }
}
