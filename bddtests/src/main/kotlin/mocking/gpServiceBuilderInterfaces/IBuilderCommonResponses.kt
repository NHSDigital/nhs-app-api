package mocking.gpServiceBuilderInterfaces

import mocking.models.Mapping

interface IBuilderCommonResponses {

    fun respondWithCorrupted(): Mapping

    fun respondWithUnavailableException(): Mapping

    fun respondWithUnknownException(): Mapping
}