package mocking.throttling

import mocking.models.Mapping
import org.apache.http.HttpStatus

class BrotherMailerResultBuilder
    : BrotherMailerMappingBuilder("POST") {

    fun respondWithOkResponse(): Mapping {
        return respondWith(HttpStatus.SC_MOVED_TEMPORARILY) {
            andHeader("Location","result=success")
                    .andBody("text", "text/plain")
                    .build()
        }
    }

    fun respondWithNotFoundError(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST){}
    }
}