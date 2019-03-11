package mocking.spine.pds

import mocking.models.Mapping
import org.apache.http.HttpStatus

class PdsNominatedPharmacyBuilder
    : PdsMappingBuilder() {

    fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(body)
        }
    }
}