package mocking.emis.me

import com.google.gson.FieldNamingPolicy
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import constants.ErrorResponseCodeEmis
import mocking.CONTENT_TYPE_APPLICATION_JSON
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.BadRequestResponse
import mocking.emis.models.LinkApplicationRequestData
import mocking.emis.models.LinkApplicationResponse
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus.SC_BAD_REQUEST
import org.apache.http.HttpStatus.SC_OK

class EmisMeBuilder(configuration: EmisConfiguration,
                    method: String,
                    patient: Patient)
    : EmisMappingBuilder(configuration, method, relativePath = "/me") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, patient.endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, patient.sessionId)

        if (method == "POST") {
            var gsonBuilder = GsonBuilder()
            gsonBuilder.setFieldNamingPolicy(FieldNamingPolicy.IDENTITY)
            val gson: Gson = gsonBuilder.create()

            val requestBody = LinkApplicationRequestData(patient)
            requestBuilder
                    .andHeader("Content-Type", CONTENT_TYPE_APPLICATION_JSON)
                    .andJsonBody(gson.toJson(requestBody))
        }
    }

    fun respondWithSuccess(accessIdentityGuid: String): Mapping {
        val responseBody = LinkApplicationResponse(accessIdentityGuid)

        return respondWith(SC_OK) {
            andJsonBody(responseBody, GsonFactory.asPascal)
                    .build()
        }
    }

    fun respondWithBadRequest(message: String): Mapping {
        val responseBody = BadRequestResponse(message)

        return respondWith(SC_BAD_REQUEST) {
            andJsonBody(responseBody, GsonFactory.asPascal)
                    .build()
        }
    }

    fun respondWithNoOnlineUserFound(): Mapping {
        return respondWithException(ErrorResponseCodeEmis.INTERNAL_ERROR.toInt(),
                                    "No registered online user found for given linkage details")
    }

    fun respondWithUserAlreadyLinked(): Mapping {
        return respondWithException(ErrorResponseCodeEmis.INTERNAL_ERROR.toInt(),
                                    "Registered online user is already linked")
    }

    fun respondWithInvalidLinkLevel(): Mapping {
        return respondWithException(ErrorResponseCodeEmis.SERVICE_ACCESS_VIOLATION.toInt(),
                                    "User Identity '00000000-0000-0000-0000-000000000000' " +
                                    "requested access level 'Linked' " +
                                    "from Application '00000000-0000-0000-0000-000000000000'. " +
                                    "Actual access level is 'Restricted'. Extra info: " +
                                    "Invalid UserApplication link level")
    }
}
