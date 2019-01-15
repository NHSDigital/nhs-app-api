package mocking.organDonation

import mocking.models.Mapping
import mocking.organDonation.models.OrganDonationRegistrationDecision
import mocking.organDonation.models.OrganDonationRegistrationRequest
import org.apache.http.HttpStatus
import org.junit.Assert

class OrganDonationSubmitDecisionBuilder(registration: OrganDonationRegistrationRequest)
    : OrganDonationMappingBuilder("POST", relativePath = "/Registration") {

    init {

        Assert.assertNotNull(registration)
        requestBuilder
                .andHeader(HEADER_SUBSCRIPTION_KEY, HEADER_SUBSCRIPTION_VALUE)
                .andHeader(HEADER_CLIENT_ID_KEY, HEADER_CLIENT_ID_VALUE)
                .andBody(mapOpt(registration.registration.decision), "contains")
    }

    private fun mapOpt(decision: OrganDonationRegistrationDecision):String{
        if(decision == OrganDonationRegistrationDecision.OptOut){
            return "opt-out"
        }
        return "opt-in"
    }

    fun respondWithSuccess(id :String): Mapping {
        val responseBody = OrganDonationRegistrationResponse(id = id)
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody).build()
        }
    }
}