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

class OrganDonationResultBuilder
    : OrganDonationMappingBuilder("POST") {

    val errorResponseCodingSystem = "http://www.nhsbt.nhs.uk/fhir/STU3/ValueSet/ODR-ErrorOrWarningCode-1"

    fun respondWithSuccess(patient: Patient): Mapping {
        val responseBody = OrganDonationSuccessResponse(
                OrganDonationRegistration.getOrganDonationRegistrationData(patient)
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
                "No ODR registration foundfor NHS number0123456789.")
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
}
