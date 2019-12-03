package mocking.emis.documents

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisDocumentsBuilder(configuration: EmisConfiguration,
                           linkToken: String,
                           apiEndUserSessionId: String,
                           apiSessionId: String)
    : EmisMappingBuilder(configuration, "GET", "/record") {

    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
                .andQueryParameter("itemType", "Documents", "equalTo")
    }

    fun respondWithNullPageCount(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(
                    "{\n" +
                            " \"MedicalRecord\": {\n" +
                            " \"Documents\":[{\"DocumentGuid\":\"" +
                            "document-1\",\"" +
                            "Observation\":" +
                            "{\"ObservationType\":\"Document\",\"Episodicity\":\"Unknown\",\"" +
                            "NumericValue\":null,\"NumericOperator\":null,\"NumericUnits\":null,\"" +
                            "DisplayValue\":null,\"TextValue\":null,\"" +
                            "Range\":null,\"Abnormal\":false,\"AbnormalReason\":null,\"" +
                            "AssociatedText\":null,\"" +
                            "EventGuid\":\"00000000-0000-0000-0000-000000000000\",\"Term\":\"" +
                            "Letter 1\",\"" +
                            "AvailabilityDateTime\":\"2018-02-18T12:44:13.187\",\"EffectiveDate\"" +
                            ":{\"DatePart\":\"" +
                            "YearMonthDay\", \"Value\":\"2018-02-18T12:44:13.187\"},\"CodeId\"" +
                            ":00000000000000, \"" +
                            "AuthorisingUserInRoleGuid\":\"00000000-0000-0000-0000-000000000000\",\"" +
                            "EnteredByUserInRoleGuid\":\"00000000-0000-0000-0000-000000000000\"},\"" +
                            "Size\":1000000, \"" +
                            "PageCount\":null,\"Extension\":\"pdf\", \"Available\":true}]}}")
        }
    }

    fun respondWithSuccess(documentsResponse: DocumentsResponseModel): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(documentsResponse)
                    .build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_FORBIDDEN.toLong(),
                "Requested record access is disabled by the practice")
        return respondWithException(exceptionResponse, HttpStatus.SC_FORBIDDEN )
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse, statusCode: Int): Mapping {
        return respondWith(statusCode) {
            andJsonBody(exceptionResponse, GsonFactory.asPascal)
        }
    }
}