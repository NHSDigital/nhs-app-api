package mocking.tpp

import constants.ErrorResponseCodeTpp
import mocking.JSonXmlConverter
import mocking.MappingBuilder
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import mocking.tpp.appointments.TppConfig
import mocking.tpp.models.Error
import org.apache.http.HttpStatus
import java.util.*

abstract class TppMappingBuilder(method: String = "POST", relativePath: String = "/tpp/") :
        IErrorMappingBuilder, MappingBuilder(method, relativePath) {

    companion object {
        const val apiVersion = "1"
        const val uuid = "3e3d8bef-4ce1-4925-a263-149c15ac7208"
        const val HEADER_CONTENT_TYPE: String = "Content-Type"
        internal const val HEADER_TYPE = "type"
        internal const val HEADER_SUID = "suid"
    }

    init {
        requestBuilder.andHeader(HEADER_CONTENT_TYPE, "text/xml; charset=UTF-8")
    }


    fun responseErrorWhenGPDisabledAppointmentsService(): Mapping {
        val errorMsg = "You don't have access to this online service"
        val disabledTppError = Error(errorCode = ErrorResponseCodeTpp.NO_ACCESS,
                userFriendlyMessage = errorMsg,
                uuid = UUID.randomUUID().toString())
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(JSonXmlConverter.toXML(disabledTppError))
        }
    }

    fun respondWithCorruptedContent(content: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(content.replace(">", "|")
                    .replace("}", "|"), contentType = "text/xml")
        }
    }

    fun respondWithTppUnknownError(errorText:String) :Mapping {
        val error = Error(ErrorResponseCodeTpp.UNKNOWN_ERROR, errorText, TppConfig.uuid)
        return respondWith(error)
    }

    fun respondWithTppNotAuthorised(errorText:String) :Mapping {
        val error = Error(ErrorResponseCodeTpp.NOT_AUTHENTICATED, errorText, TppConfig.uuid)
        return respondWith(error)
    }

    protected inline fun <reified T : Any> respondWith(response: T): Mapping {

        val xmlBody = JSonXmlConverter.toXML(response)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(xmlBody)
                    .andHeader("type", "")
                    .andDelay(delayMillisecs)
                    .build()
        }
    }

    override fun respondWithError(httpStatusCode: Int, errorCode: String, message: String?): Mapping {
        val disabledTppError = Error(errorCode = errorCode, userFriendlyMessage = message?:"")
        return respondWith(httpStatusCode) {
            andXmlBody(JSonXmlConverter.toXML(disabledTppError))
        }
    }
}
