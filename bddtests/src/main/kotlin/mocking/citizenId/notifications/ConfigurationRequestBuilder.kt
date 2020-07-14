package mocking.citizenId.notifications

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.citizenId.models.notifications.SuccessResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

open class ConfigurationRequestBuilder
    : CitizenIdMappingBuilder("GET", "/.well-known/openid-configuration") {

    fun respondWithSuccess(
            response: SuccessResponse): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
        }
    }
}
