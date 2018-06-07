package mocking.emis.session

import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.EndUserSessionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus


class EmisEndUserSessionBuilder(configuration: EmisConfiguration)
    : EmisMappingBuilder(configuration, "POST", "/sessions/endusersession") {
    init {
        // no extra params required
    }

    fun respondWithSuccess(endUserSessionId: String, milliSecondDelay: Int = 0): Mapping {

        val responseBody = EndUserSessionResponse(endUserSessionId)
        return respondWith(HttpStatus.SC_OK, milliSecondDelay) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun respondWithServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
    }

    fun respondWithServiceUnavailable(): Mapping {
        val responseBody = "Service Unavailable"
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andJsonBody(responseBody)
            build()
        }
    }
}
