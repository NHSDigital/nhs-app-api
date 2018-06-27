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
        return respondWithException(-1030, "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'RepeatPrescribing' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. Available services are 'AddressChange, AppointmentBooking, RecordViewer, SharedRecordAuditView'. Extra info: Services Access violation")
    }

}
