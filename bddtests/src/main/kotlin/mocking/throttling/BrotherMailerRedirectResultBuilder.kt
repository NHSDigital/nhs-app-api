package mocking.throttling

import mocking.models.Mapping
import org.apache.http.HttpStatus

class BrotherMailerRedirectResultBuilder: BrotherMailerRedirectMappingBuilder("GET") {

    fun respondWithSuccess(): Mapping {
        requestBuilder.andQueryParameter("result", "success")

        return respondWith(HttpStatus.SC_OK) {
        }
    }

    fun respondWithInvalidEmail(): Mapping {
        requestBuilder.andQueryParameter("reason", "invalidemail")
        requestBuilder.andQueryParameter("result", "unsuccessful")

        return respondWith(HttpStatus.SC_OK) {
        }
    }
}