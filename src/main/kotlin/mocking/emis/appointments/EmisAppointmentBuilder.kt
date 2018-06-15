package mocking.emis.appointments

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisAppointmentBuilder(configuration: EmisConfiguration, postAppointmentRequest: PostAppointmentRequestModel) :
        EmisMappingBuilder(configuration, "POST", "/appointments") {

    init {
        requestBuilder.andJsonBody(postAppointmentRequest, gson = GsonFactory.asPascal)
    }

    fun respondWithSuccess(response: PostAppointmentResponseModel): Mapping {
        return respondWithSuccessAny(response)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_CREATED) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}