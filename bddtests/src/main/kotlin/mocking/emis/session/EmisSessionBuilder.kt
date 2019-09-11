package mocking.emis.session

import constants.ErrorResponseCodeEmis
import mocking.CONTENT_TYPE_APPLICATION_JSON
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.AssociationType
import mocking.emis.models.UserPatientLink
import mocking.models.Mapping
import models.Patient
import models.patients.EmisPatients
import org.apache.http.HttpStatus


class EmisSessionBuilder(configuration: EmisConfiguration,
                         patient: Patient)
    : EmisMappingBuilder(configuration, "POST", "/sessions") {
    init {
        val bodyProperties = CreateSessionRequestModel(patient.connectionToken, patient.odsCode)

        requestBuilder
                .andHeader("Content-Type", CONTENT_TYPE_APPLICATION_JSON)
                .andJsonBody(bodyProperties)
    }

    fun respondWithSuccess(patient: Patient, associationType: AssociationType): Mapping {
        val responseBody = CreateSessionResponseModel(
                title = patient.title,
                firstName = patient.firstName,
                surname = patient.surname,
                sessionId = patient.sessionId,
                userPatientLinkToken = patient.userPatientLinkToken,
                odsCode = patient.odsCode,
                associationType = associationType)

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody, GsonFactory.asPascal)
                    .build()
        }
    }

    fun respondWithSuccessForLinkedPatients(patient: Patient, associationType: AssociationType): Mapping {
        val responseBody = CreateSessionResponseModel(
                title = patient.title,
                firstName = patient.firstName,
                surname = patient.surname,
                sessionId = patient.sessionId,
                userPatientLinkToken = patient.userPatientLinkToken,
                odsCode = patient.odsCode,
                associationType = associationType)

        val linkedPatient = EmisPatients.johnSmith

        val userPatientLink = UserPatientLink(
                title = linkedPatient.title,
                firstName = linkedPatient.firstName,
                surname = linkedPatient.surname,
                userPatientLinkToken = linkedPatient.userPatientLinkToken,
                odsCode = linkedPatient.odsCode,
                associationType =  AssociationType.Proxy
        )

        responseBody.userPatientLinks.add(userPatientLink)

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody, GsonFactory.asPascal)
                    .build()
        }
    }

    fun respondWithUserNotRegistered(): Mapping {
        return respondWithException(ErrorResponseCodeEmis.SERVICE_ACCESS_VIOLATION.toInt(),
                                    "User Identity '00000000-0000-0000-0000-000000000000' " +
                                    "required account status 'Inactive, Active' from Application " +
                                    "'00000000-0000-0000-0000-000000000000'. Actual account status is " +
                                    "'NotRegistered'. Extra info: Invalid account registration status")
    }

    fun respondWithServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
    }

    fun respondWithForbidden(): Mapping {
        return respondWith(HttpStatus.SC_FORBIDDEN) { build() }
    }
}