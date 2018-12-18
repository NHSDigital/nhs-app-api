package mocking.organDonation

import config.Config
import mocking.MappingBuilder

const val HEADER_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key"
const val HEADER_CLIENT_ID = "X-Client-ID"

open class OrganDonationMappingBuilder(method: String)
    : MappingBuilder(method, Config.instance.updatedOrganDonation) {

    init {
        requestBuilder
                .andHeader(HEADER_SUBSCRIPTION_KEY, "781859de3cce4bc48a715dfc8f36dc6f")
                .andHeader(HEADER_CLIENT_ID, "ODR-26-NHSAP")
    }

    fun postToOrganDonation() = OrganDonationResultBuilder()
}
