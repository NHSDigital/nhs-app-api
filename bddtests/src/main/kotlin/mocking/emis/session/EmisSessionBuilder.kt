package mocking.emis.session

import mocking.CONTENT_TYPE_APPLICATION_JSON
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.models.AssociationType
import mocking.models.Mapping
import models.Patient
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

    fun respondWithUserNotRegistered(): Mapping {
        return respondWithException(-1030, "User Identity '00000000-0000-0000-0000-000000000000' required account status 'Inactive, Active' from Application '00000000-0000-0000-0000-000000000000'. Actual account status is 'NotRegistered'. Extra info: Invalid account registration status")
    }

    fun respondWithServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
    }

    fun respondWithForbidden(): Mapping {
        return respondWith(HttpStatus.SC_FORBIDDEN) { build() }
    }
}