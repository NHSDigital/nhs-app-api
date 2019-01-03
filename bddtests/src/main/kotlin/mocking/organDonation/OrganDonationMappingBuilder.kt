package mocking.organDonation

import config.Config
import mocking.MappingBuilder
import models.Patient

const val HEADER_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key"
const val HEADER_SUBSCRIPTION_VALUE = "781859de3cce4bc48a715dfc8f36dc6f"

const val HEADER_CLIENT_ID_KEY = "X-Client-ID"
const val HEADER_CLIENT_ID_VALUE = "ODR-26-NHSAP"

open class OrganDonationMappingBuilder(method: String, relativePath: String = "")
    : MappingBuilder(method, Config.instance.updatedOrganDonation + relativePath) {

    fun lookupOrganDonationRegistration(patient: Patient) : OrganDonationLookupRegistrationBuilder {
        return OrganDonationLookupRegistrationBuilder(patient)
    }
}
