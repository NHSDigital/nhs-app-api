package mocking.citizenId.login

import mocking.GsonFactory
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.citizenId.models.UserInfo
import mocking.citizenId.models.GpIntegrationCredentials
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class UserInfoRequestBuilder(
        accessToken: String
) : CitizenIdMappingBuilder("GET", "/userinfo") {
    init {
        requestBuilder
                .andHeader("Authorization", "Bearer $accessToken", "contains")
    }

    fun respondWithSuccess(patient: Patient): Mapping {

        val im1ConnectionToken = if (patient.im1ConnectionToken == null) {
            patient.connectionToken
        } else {
            GsonFactory.asPascal.toJson(patient.im1ConnectionToken)
        }

        val userInfo = UserInfo(
                NhsNumber = patient.nhsNumbers.firstOrNull() ?: "",
                Birthdate = patient.dateOfBirth,
                Im1ConnectionToken = im1ConnectionToken,
                GpIntegrationCredentials = GpIntegrationCredentials(patient.odsCode),
                GivenName = patient.firstName,
                FamilyName = patient.surname,
                Subject = patient.subject
        )

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(userInfo)
        }
    }

    fun respondWithServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
    }
}
