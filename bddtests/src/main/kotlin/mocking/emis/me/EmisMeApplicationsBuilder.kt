package mocking.emis.me

import constants.EmisResponseCode
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisMeApplicationsBuilder(configuration: EmisConfiguration,
                                endUserSessionId: String,
                                model: LinkApplicationRequestModel):
    EmisMappingBuilder(configuration, "POST", "/me/applications"){

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, endUserSessionId)
                .andJsonBody(model, gson = GsonFactory.asPascal)
    }

    fun respondWithSuccess(accessIdentityGuid: String): Mapping {
        return respondWith(HttpStatus.SC_CREATED){
            andJsonBody(LinkApplicationResponseModel(accessIdentityGuid = accessIdentityGuid), GsonFactory.asPascal)
        }
    }

    fun respondWithAlreadyLinked(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(ExceptionResponse(
                internalResponseCode = EmisResponseCode.INTERNAL_ERROR,
                exceptionMessage = "Registered online user is already linked"
            ), GsonFactory.asPascal)
        }
    }

    fun respondWithLinkageKeyDoesNotMatch(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(ExceptionResponse(
                    internalResponseCode = EmisResponseCode.INTERNAL_ERROR,
                    exceptionMessage = "Invalid linkage details"
            ), GsonFactory.asPascal)
        }
    }

    fun respondWithIncorrectSurnameOrDateOfBirth(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(ExceptionResponse(
                    internalResponseCode = EmisResponseCode.INTERNAL_ERROR,
                    exceptionMessage = "No match found for given demographics"
            ), GsonFactory.asPascal)
        }
    }

    fun respondWithNoOnlineUserFound(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(ExceptionResponse(
                internalResponseCode = EmisResponseCode.INTERNAL_ERROR,
                exceptionMessage = "No registered online user found for given linkage details"
            ), GsonFactory.asPascal)
        }
    }
}