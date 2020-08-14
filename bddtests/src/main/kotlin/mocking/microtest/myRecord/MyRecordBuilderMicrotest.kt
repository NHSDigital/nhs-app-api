package mocking.microtest.myRecord

import mocking.GsonFactory
import mocking.models.ExceptionResponse
import mocking.microtest.HEADER_API_NHS_NUMBER
import mocking.microtest.HEADER_API_ODS_CODE
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_OK

class MyRecordBuilderMicrotest(odsCode: String,
                               nhsNumber: String)
    : MicrotestMappingBuilder( "GET", "/my-record") {

    init {
        requestBuilder
                .andHeader(HEADER_API_ODS_CODE, odsCode)
                .andHeader(HEADER_API_NHS_NUMBER, nhsNumber)
    }

    fun respondWithSuccess(model: MyRecordResponseModel): Mapping {
        return respondWith(SC_OK) {
            andJsonBody(model)
                    .build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_INTERNAL_SERVER_ERROR.toLong(),
                "Requested record access is disabled by the practice")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}
