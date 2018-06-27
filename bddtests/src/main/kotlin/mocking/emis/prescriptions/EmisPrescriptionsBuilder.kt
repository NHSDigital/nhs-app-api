package mocking.emis.prescriptions

import mocking.GsonFactory
import mocking.emis.*
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import java.time.OffsetDateTime

private const val QUERY_PRESCRIPTION_FROMDATE = "filterFromDate"
private const val QUERY_PRESCRIPTION_TODATE = "filterToDate"

class EmisPrescriptionsBuilder (configuration: EmisConfiguration,
                                apiEndUserSessionId: String,
                                apiSessionId: String,
                                linkToken: String?,
                                fromDate: OffsetDateTime?,
                                toDate: OffsetDateTime?)
    :EmisMappingBuilder(configuration, "GET", "/prescriptionrequests"){

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if(linkToken != null) {
            requestBuilder.andQueryParameter(QUERY_PARAM_USER_PATIENT_LINK_TOKEN, linkToken,"equalTo")
        }

        if(fromDate != null) {
            requestBuilder.andQueryParameter(QUERY_PRESCRIPTION_FROMDATE, getDateFormattedString(fromDate), "contains")
        }

        if(toDate != null) {
            requestBuilder.andQueryParameter(QUERY_PRESCRIPTION_TODATE, getDateFormattedString(toDate), "contains" )
        }
    }

    fun respondWithSuccess(prescriptionRequestsGetResponse: PrescriptionRequestsGetResponse): Mapping {
        return respondWithSuccessAny(prescriptionRequestsGetResponse)
    }

    fun respondWithPrescriptionsNotEnabled(): Mapping {
        return respondWithException(-1030, "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'RepeatPrescribing' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. Available services are 'AddressChange, AppointmentBooking, RecordViewer, SharedRecordAuditView'. Extra info: Services Access violation")
    }

    private fun getDateFormattedString(dateTime: OffsetDateTime): String{
        return String.format("%s-%s-%s", dateTime.year, formatDateToTwoDigits(dateTime.monthValue), formatDateToTwoDigits(dateTime.dayOfMonth))
    }

    private fun formatDateToTwoDigits(daysOrMonths: Int): String{
        return String.format("%02d", daysOrMonths)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}