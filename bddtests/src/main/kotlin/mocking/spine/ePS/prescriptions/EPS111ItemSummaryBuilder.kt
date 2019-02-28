package mocking.spine.ePS.prescriptions

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.spine.ePS.EPSMappingBuilder
import mocking.spine.ePS.models.SpineItemSummaryGetResponse
import java.time.OffsetDateTime
import mocking.models.Mapping
import org.apache.http.HttpStatus

private const val QUERY_NHS_NUMBER = "nhsNumber"
private const val QUERY_FROM_DATE = "earliestDate"
private const val QUERY_TO_DATE = "latestDate"
private const val QUERY_PRESCRIPTION_STATUS = "prescriptionStatus"
private const val QUERY_PRESCRIPTION_VERSION = "prescriptionVersion"
private const val QUERY_VERSION = "version"

class EPS111ItemSummaryBuilder (
                                   fromDate: OffsetDateTime?,
                                   toDate: OffsetDateTime?,
                                   prescriptionStatus: String?,
                                   prescriptionVersion: String?,
                                   version: String?
                                       )
    : EPSMappingBuilder( "GET", "/nhs111itemsummary" ) {

    init {
        if (fromDate != null) {
            requestBuilder.andQueryParameter(QUERY_FROM_DATE, getDateFormattedString(fromDate))
        }

        if (toDate != null) {
            requestBuilder.andQueryParameter(QUERY_TO_DATE, getDateFormattedString(toDate))
        }

        requestBuilder.andQueryParameterIfNotNull(QUERY_PRESCRIPTION_STATUS, prescriptionStatus)
        requestBuilder.andQueryParameterIfNotNull(QUERY_PRESCRIPTION_VERSION, prescriptionVersion)
        requestBuilder.andQueryParameterIfNotNull(QUERY_VERSION, version)
    }

    fun respondWithSuccess(spineItemSummaryGetResponse: SpineItemSummaryGetResponse): Mapping {
        return respondWithSuccessAny(spineItemSummaryGetResponse)
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
            andJsonBody(body, GsonBuilder().setFieldNamingPolicy(FieldNamingPolicy.IDENTITY).create())
        }
    }
}