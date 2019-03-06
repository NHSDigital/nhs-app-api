package mocking.organDonation

import config.Config
import mocking.MappingBuilder
import mocking.models.Mapping
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.Issue
import mocking.organDonation.models.OrganDonationRegistrationRequest
import models.Patient

const val HEADER_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key"
const val HEADER_SUBSCRIPTION_VALUE = "781859de3cce4bc48a715dfc8f36dc6f"

const val HEADER_CLIENT_ID_KEY = "X-Client-ID"
const val HEADER_CLIENT_ID_VALUE = "ODR-26-NHSAP"

const val ORGAN_DONATION_ERROR_CODE_UPDATE_CONFLICT = 10201

open class OrganDonationMappingBuilder(method: String, relativePath: String = "")
    : MappingBuilder(method, Config.instance.updatedOrganDonation + relativePath) {
    protected val errorResponseCodingSystem = "http://www.nhsbt.nhs.uk/fhir/STU3/ValueSet/ODR-ErrorOrWarningCode-1"

    fun lookupOrganDonationRegistration(patient: Patient) : OrganDonationLookupRegistrationBuilder {
        return OrganDonationLookupRegistrationBuilder(patient)
    }

    fun referenceData() = OrganDonationReferenceDataBuilder()

    fun submitDecision(registration: OrganDonationRegistrationRequest) =
            OrganDonationSubmitDecisionBuilder(registration, "POST", "/Registration")

    fun amendDecision(registration: OrganDonationRegistrationRequest) =
            OrganDonationSubmitDecisionBuilder(registration, "PUT", "/Registration/"
                    + registration.registration.identifier)

    fun respondWithError(httpStatus: Int) : Mapping {
        val responseBody = listOf(Issue(
                details = CodeableConcept(
                        listOf(Coding(
                                errorResponseCodingSystem,
                                "")))))
        return respondWith(httpStatus) {
            andJsonBody(responseBody).build()
        }
    }
}
