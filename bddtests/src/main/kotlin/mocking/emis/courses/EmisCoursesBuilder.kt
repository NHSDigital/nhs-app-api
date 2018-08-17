package mocking.emis.courses

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.GsonFactory
import mocking.emis.*
import mocking.emis.models.CourseRequestsGetResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisCoursesBuilder(configuration: EmisConfiguration,
                         apiEndUserSessionId: String,
                         apiSessionId: String,
                         linkToken: String?)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/courses") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (!linkToken.isNullOrEmpty()) {
            requestBuilder.andQueryParameter(QUERY_PARAM_USER_PATIENT_LINK_TOKEN, linkToken!!, "equalTo")
        }
    }

    fun respondWithSuccess(courseRequestsGetResponse: CourseRequestsGetResponse): Mapping {

        return respondWithSuccessAny(courseRequestsGetResponse)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }

    fun respondWithPrescriptionsNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

}
