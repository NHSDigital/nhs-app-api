package mocking.qualtrics

import config.Config
import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class QualtricsMappingBuilder : MappingBuilder("POST",
        "/qualtrics/directories/${Config.instance.qualtricsDirectoryId}" +
                "/mailinglists/${Config.instance.qualtricsMailingList}/contacts") {

    fun respondWithSuccess(): Mapping {
        return respondWith(HttpStatus.SC_OK) { build() }
    }

    fun respondWithServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
    }
}
