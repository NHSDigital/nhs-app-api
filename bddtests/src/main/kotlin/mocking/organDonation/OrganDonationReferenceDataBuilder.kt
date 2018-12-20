package mocking.organDonation

import mocking.models.Mapping
import mocking.organDonation.models.OrganDonationSuccessResponse
import mocking.organDonation.models.ReferenceDataResponse
import org.apache.http.HttpStatus

class OrganDonationReferenceDataBuilder : OrganDonationMappingBuilder("GET", relativePath = "/ReferenceData") {

    fun respondWithSuccess(response: OrganDonationSuccessResponse<ReferenceDataResponse>): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                    .build()
        }
    }
}
