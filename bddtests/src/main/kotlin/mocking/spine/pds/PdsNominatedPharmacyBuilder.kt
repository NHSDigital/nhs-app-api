package mocking.spine.pds

import mocking.models.Mapping
import org.apache.http.HttpStatus

class PdsNominatedPharmacyBuilder(soapAction: String)
    : PdsMappingBuilder(soapAction = soapAction) {

    fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(body)
        }
    }

    fun respondWithAccepted(): Mapping {
        return respondWith(HttpStatus.SC_ACCEPTED) {
        }
    }
}