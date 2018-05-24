package mocking.favicon

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class FaviconMappingBuilder : MappingBuilder("GET", "/favicon.ico") {

    fun respondWithNotFound(): Mapping {
        return respondWith(HttpStatus.SC_NOT_FOUND) {}
    }
}