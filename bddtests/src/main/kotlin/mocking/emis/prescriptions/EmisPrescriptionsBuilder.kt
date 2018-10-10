package mocking.emis.prescriptions

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.QUERY_PARAM_USER_PATIENT_LINK_TOKEN
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import java.time.OffsetDateTime

private const val QUERY_PRESCRIPTION_FROM_DATE = "filterFromDate"
private const val QUERY_PRESCRIPTION_TO_DATE = "filterToDate"

class  EmisPrescriptionsBuilder (configuration: EmisConfiguration,
                                apiEndUserSessionId: String,
                                apiSessionId: String,
                                linkToken: String?,
                                fromDate: OffsetDateTime?,
                                toDate: OffsetDateTime?)
    : EmisMappingBuilder(configuration, "GET", "/prescriptionrequests") {

    lateinit var data:PrescriptionRequestsGetResponse
    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if(linkToken != null) {
            requestBuilder.andQueryParameter(QUERY_PARAM_USER_PATIENT_LINK_TOKEN, linkToken,"equalTo")
        }

        if(fromDate != null) {
            requestBuilder.andQueryParameter(QUERY_PRESCRIPTION_FROM_DATE, getDateFormattedString(fromDate), "contains")
        }

        if(toDate != null) {
            requestBuilder.andQueryParameter(QUERY_PRESCRIPTION_TO_DATE, getDateFormattedString(toDate), "contains" )
        }
    }

    fun respondWithSuccess(prescriptionRequestsGetResponse: PrescriptionRequestsGetResponse): Mapping {
        return respondWithSuccessAny(prescriptionRequestsGetResponse)
    }

    fun respondWithPrescriptionsNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    private fun getDateFormattedString(dateTime: OffsetDateTime): String {
        return String.format("%s-%s-%s", dateTime.year,
                             formatDateToTwoDigits(dateTime.monthValue),
                             formatDateToTwoDigits(dateTime.dayOfMonth))
    }

    private fun formatDateToTwoDigits(daysOrMonths: Int): String {
        return String.format("%02d", daysOrMonths)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}
