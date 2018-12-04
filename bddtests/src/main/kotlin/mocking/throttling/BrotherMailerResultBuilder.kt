package mocking.throttling

import config.Config
import mocking.models.Mapping
import org.apache.http.HttpStatus

class BrotherMailerResultBuilder
    : BrotherMailerMappingBuilder("POST") {

    fun respondWithOkResponse(): Mapping {
        return redirectTo(
                "${Config.instance.brotherMailerRedirectPath}?result=success")
    }

    fun respondWithNotFoundError(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST){}
    }
}