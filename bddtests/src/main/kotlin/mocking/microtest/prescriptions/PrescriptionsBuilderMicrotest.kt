package mocking.microtest

import mocking.GsonFactory
import mocking.microtest.prescriptions.PrescriptionHistoryGetResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import java.time.OffsetDateTime

private const val QUERY_PRESCRIPTION_FROM_DATE = "from_date"

class PrescriptionsBuilderMicrotest (
        nhsNumber: String,
        odsCode: String,
        fromDate: OffsetDateTime?)
    : MicrotestMappingBuilder("GET", "/prescriptions") {

    init {
        requestBuilder
                .andHeader(HEADER_API_ODS_CODE, odsCode)
                .andHeader(HEADER_API_NHS_NUMBER, nhsNumber)

        if(fromDate != null) {
            requestBuilder.andQueryParameter(QUERY_PRESCRIPTION_FROM_DATE, getDateFormattedString(fromDate), "contains")
        }
    }

    fun respondWithSuccess(prescriptionRequestsGetResponse: PrescriptionHistoryGetResponse): Mapping {
        return respondWithSuccessAny(prescriptionRequestsGetResponse)
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
