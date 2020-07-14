package mocking.gpServiceBuilderInterfaces

import mocking.models.Mapping

interface IErrorMappingBuilder {
    fun respondWithError(httpStatusCode: Int, errorCode: String, message: String? = null): Mapping
}
