package mocking.gpServiceBuilderInterfaces

import mocking.models.Mapping

interface IBuilderCommonResponses {

    fun respondWithCorrupted(): Mapping

    fun respondWithServiceUnavailable(): Mapping

    fun respondWithUnknownException(): Mapping

    fun respondWithGPErrorWhenNotEnabled(): Mapping
}