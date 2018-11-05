package mocking.tpp.session

import constants.ErrorResponseCodeTpp
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.LogOffReply


class TppLogOffBuilder : TppMappingBuilder("POST", "/tpp/") {
    init {
        val typeHeader = "type"
        val typeValue = "Logoff"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//Logoff[" +
                        "@apiVersion='${apiVersion}' and " +
                        "@uuid='${uuid}']")
    }

    fun respondWithSuccess(): Mapping {
        val responseBody = LogOffReply(uuid)
        return respondWith(responseBody)
    }

    fun respondWithError(): Mapping {
        val error = Error(ErrorResponseCodeTpp.NOT_LOGGED_IN, "You must be logged on" , uuid)
        return respondWith(error)
    }
}