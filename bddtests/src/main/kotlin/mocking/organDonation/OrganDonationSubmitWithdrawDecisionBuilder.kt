package mocking.organDonation

import mocking.models.Mapping
import mocking.organDonation.models.OrganDonationWithdrawRequest
import org.apache.http.HttpStatus
import org.junit.Assert

class OrganDonationSubmitWithdrawDecisionBuilder (
        withdrawRequestBody: OrganDonationWithdrawRequest, method: String, path: String)
    : OrganDonationMappingBuilder(method, relativePath = path) {

    init {
        Assert.assertNotNull(withdrawRequestBody)
        requestBuilder
                .andHeader(HEADER_SUBSCRIPTION_KEY, HEADER_SUBSCRIPTION_VALUE)
                .andHeader(HEADER_CLIENT_ID_KEY, HEADER_CLIENT_ID_VALUE)
                .andBody(withdrawRequestBody.WithdrawReasonId, "contains")
                .andBody(withdrawRequestBody.identifier, "contains")
    }

    fun respondWithSuccess(id: String): Mapping {
        val responseBody = OrganDonationWithdrawalResponse(id = id)
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody).build()
        }
    }
}
