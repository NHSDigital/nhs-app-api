package mocking.organDonation

import mocking.models.Mapping
import mocking.organDonation.models.Entry
import mocking.organDonation.models.OrganDonationSuccessResponse
import mocking.organDonation.models.Resource
import models.Patient
import org.apache.http.HttpStatus

class OrganDonationLookupRegistrationBuilder(patient: Patient)
    : OrganDonationMappingBuilder("POST", relativePath = "/Registration/_search") {

    init {

        requestBuilder
                .andHeader(HEADER_SUBSCRIPTION_KEY, HEADER_SUBSCRIPTION_VALUE)
                .andHeader(HEADER_CLIENT_ID_KEY, HEADER_CLIENT_ID_VALUE)
                .andBody("https://fhir.nhs.uk/Id/nhs-number|" + patient.nhsNumbers.first(), "contains")
                .andBody(patient.dateOfBirth, "contains")
    }

    fun respondWithSuccess(resource : Resource): Mapping {

        val responseBody = OrganDonationSuccessResponse(
                listOf(Entry(resource))
        )
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody)
                    .build()
        }
    }
}
