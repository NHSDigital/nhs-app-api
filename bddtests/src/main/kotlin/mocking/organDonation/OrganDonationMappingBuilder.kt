package mocking.organDonation

import config.Config
import mocking.MappingBuilder
import mocking.models.Mapping
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.OrganDonationErrorResponse
import mocking.organDonation.models.OrganDonationRegistrationRequest
import models.Patient
import org.apache.http.HttpStatus

const val HEADER_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key"
const val HEADER_SUBSCRIPTION_VALUE = "781859de3cce4bc48a715dfc8f36dc6f"

const val HEADER_CLIENT_ID_KEY = "X-Client-ID"
const val HEADER_CLIENT_ID_VALUE = "ODR-26-NHSAP"

const val ORGAN_DONATION_ERROR_CODE_NOT_FOUND = 20010
const val ORGAN_DONATION_ERROR_CODE_GATEWAY_TIMEOUT = 20022
const val ORGAN_DONATION_ERROR_CODE_CONFLICT = 10112
const val ORGAN_DONATION_ERROR_CODE_INTERNAL_SERVER_ERROR = 20120

open class OrganDonationMappingBuilder(method: String, relativePath: String = "")
    : MappingBuilder(method, Config.instance.updatedOrganDonation + relativePath) {
    protected val errorResponseCodingSystem = "http://www.nhsbt.nhs.uk/fhir/STU3/ValueSet/ODR-ErrorOrWarningCode-1"

    fun lookupOrganDonationRegistration(patient: Patient) : OrganDonationLookupRegistrationBuilder {
        return OrganDonationLookupRegistrationBuilder(patient)
    }

    fun referenceData() = OrganDonationReferenceDataBuilder()

    fun submitDecision(registration: OrganDonationRegistrationRequest) =
            OrganDonationSubmitDecisionBuilder(registration)

    fun respondWithTimeOutError() : Mapping {
        val responseBody = OrganDonationErrorResponse(
                "transient",
                CodeableConcept(
                        listOf(Coding(
                                errorResponseCodingSystem,
                                ORGAN_DONATION_ERROR_CODE_GATEWAY_TIMEOUT.toString()))),
                "Any further internal debug details i.e. stack trace details etc.")
        return respondWith(HttpStatus.SC_REQUEST_TIMEOUT) {
            andJsonBody(responseBody).build()
        }
    }

    fun respondWithInternalError() : Mapping {
        val responseBody = OrganDonationErrorResponse(
                "internal server error",
                CodeableConcept(
                        listOf(Coding(
                                errorResponseCodingSystem,
                                ORGAN_DONATION_ERROR_CODE_INTERNAL_SERVER_ERROR.toString()))),
                "Internal Server Error")
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(responseBody).build()
        }
    }
}
