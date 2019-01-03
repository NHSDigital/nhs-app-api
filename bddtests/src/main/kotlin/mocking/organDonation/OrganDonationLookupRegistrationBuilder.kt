package mocking.organDonation

import mocking.data.organDonation.OrganDonationRegistration
import mocking.models.Mapping
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.OrganDonationSuccessResponse
import mocking.organDonation.models.OrganDonationErrorResponse
import models.Patient
import org.apache.http.HttpStatus

const val ORGAN_DONATION_ERROR_CODE_NOT_FOUND = 20010
const val ORGAN_DONATION_ERROR_CODE_GATEWAY_TIMEOUT = 20022
const val ORGAN_DONATION_ERROR_CODE_CONFLICT = 10112

class OrganDonationLookupRegistrationBuilder(patient: Patient)
    : OrganDonationMappingBuilder("POST", relativePath = "/Registration/_search") {

    init {

        requestBuilder
                .andHeader(HEADER_SUBSCRIPTION_KEY, HEADER_SUBSCRIPTION_VALUE)
                .andHeader(HEADER_CLIENT_ID_KEY, HEADER_CLIENT_ID_VALUE)
                .andBody("https://fhir.nhs.uk/Id/nhs-number|" + patient.nhsNumbers.first(), "contains")
                .andBody(patient.dateOfBirth, "contains")
    }

    private val errorResponseCodingSystem = "http://www.nhsbt.nhs.uk/fhir/STU3/ValueSet/ODR-ErrorOrWarningCode-1"
    private val currentPatient = patient

    fun respondWithSuccess(): Mapping {
        val responseBody = OrganDonationSuccessResponse(
                OrganDonationRegistration.getOrganDonationRegistrationData(currentPatient)
        )

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun respondWithNotFoundError() : Mapping {
        val responseBody = OrganDonationErrorResponse(
                "not-found",
                CodeableConcept(
                        listOf(Coding(
                                errorResponseCodingSystem,
                                ORGAN_DONATION_ERROR_CODE_NOT_FOUND))),
                "No ODR registration found for NHS number.")
        return respondWith(HttpStatus.SC_NOT_FOUND) {
            andJsonBody(responseBody).build()
        }
    }

    fun respondWithTimeoutError() : Mapping {
        val responseBody = OrganDonationErrorResponse(
                "transient",
                CodeableConcept(
                        listOf(Coding(
                                errorResponseCodingSystem,
                                ORGAN_DONATION_ERROR_CODE_GATEWAY_TIMEOUT))),
                "Any further internal debug details i.e. stack trace details etc.")
        return respondWith(HttpStatus.SC_GATEWAY_TIMEOUT) {
            andJsonBody(responseBody).build()
        }
    }

    fun respondWithConflictError() : Mapping {
        val responseBody = OrganDonationErrorResponse(
                "duplicate",
                CodeableConcept(
                        listOf(Coding(
                                errorResponseCodingSystem,
                                ORGAN_DONATION_ERROR_CODE_CONFLICT))),
                "Multiple ODR registrations found for NHS number")
        return respondWith(HttpStatus.SC_CONFLICT) {
            andJsonBody(responseBody).build()
        }
    }
}
