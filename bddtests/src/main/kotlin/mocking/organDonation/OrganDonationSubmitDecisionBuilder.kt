package mocking.organDonation

import mocking.models.Mapping
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.Issue
import mocking.organDonation.models.OrganDonationRegistrationDecision
import mocking.organDonation.models.OrganDonationRegistrationRequest
import org.apache.http.HttpStatus
import org.junit.Assert

class OrganDonationSubmitDecisionBuilder(registration: OrganDonationRegistrationRequest, method: String, path: String)
    : OrganDonationMappingBuilder(method, relativePath = path) {

    init {

        Assert.assertNotNull(registration)
        requestBuilder
                .andHeader(HEADER_SUBSCRIPTION_KEY, HEADER_SUBSCRIPTION_VALUE)
                .andHeader(HEADER_CLIENT_ID_KEY, HEADER_CLIENT_ID_VALUE)
                .andBody(mapOpt(registration.registration.decision), "contains")
    }

    private fun mapOpt(decision: OrganDonationRegistrationDecision): String {
        if (decision == OrganDonationRegistrationDecision.OptOut) {
            return "opt-out"
        }
        return "opt-in"
    }

    fun respondWithSuccess(id: String): Mapping {
        val responseBody = OrganDonationRegistrationResponse(id = id)
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody).build()
        }
    }

    fun respondWithError(id: String, httpStatus: Int): Mapping {
        val responseBody = OrganDonationRegistrationResponse(
                id = id)
        return respondWith(httpStatus) {
            andJsonBody(responseBody).build()
        }
    }

    fun respondWithConflict(id: String, errorCode: String): Mapping {
        val responseBody = OrganDonationRegistrationResponse(
                id = id,
                issue = listOf(Issue(
                        details = CodeableConcept(
                                arrayListOf(Coding( errorCode,errorResponseCodingSystem))))))
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody).build()
        }
    }
}
