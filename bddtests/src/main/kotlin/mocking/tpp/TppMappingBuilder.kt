package mocking.tpp

import mocking.JSonXmlConverter
import mocking.MappingBuilder
import mocking.models.Mapping
import mocking.tpp.models.Error
import org.apache.http.HttpStatus
import java.util.*

open class TppMappingBuilder(method: String = "POST", relativePath: String = "/tpp/") :
        MappingBuilder(method, relativePath) {

    private val HEADER_CONTENT_TYPE = "Content-Type"
    internal val HEADER_TYPE = "type"
    internal val HEADER_SUID = "suid"

    var delayMillisecs = 0

    init {
        requestBuilder.andHeader(HEADER_CONTENT_TYPE, "text/xml; charset=UTF-8")
    }

    val appointments = TppMappingBuilderAppointments()

    var myRecord = TppMappingBuilderMyRecord()

    var prescriptions = TppMappingBuilderPrescriptions()

    var authentication = TppMappingBuilderAuthentication()


    fun responseErrorWhenGPDisabledAppointmentsService(): Mapping {
        val errorMsg = "You don't have access to this online service"
        val disabledTppError = Error(errorCode = "6", userFriendlyMessage = errorMsg,
                                     uuid = UUID.randomUUID().toString())
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(JSonXmlConverter.toXML(disabledTppError))
        }
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

    companion object {
        const val apiVersion = "1"
        const val uuid = "3e3d8bef-4ce1-4925-a263-149c15ac7208"
    }

}
