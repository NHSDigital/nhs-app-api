package features.myrecord.factories

import constants.ErrorResponseCodeTpp
import constants.TppConstants
import mocking.data.myrecord.TestResultsData
import mocking.tpp.models.Error
import models.Patient
import worker.models.myrecord.TestResultItem
import java.time.Duration
import java.time.OffsetDateTime
import java.time.ZoneOffset
import kotlin.math.floor

private const val START_DATE_FOR_RANGE_ONE = 179L
private const val END_DATE_FOR_RANGE_ONE = 120L
private const val MAX_MINUTE_SECONDS =  59
private const val MAX_HOUR =  23
private const val MAX_MONTH =  12
private const val MAX_DAY_INT =  31
private const val MAX_DAYS_IN_REQUEST =  60

class TestResultsFactoryTpp : TestResultsFactory(){
    override fun respondWithACorruptedResponse(patient: Patient) {
        val today = OffsetDateTime.now()
        val startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        val endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp.mock {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithCorruptedContent("Bad Data")
        }
    }

    override fun disabled(patient: Patient) {
        val today = OffsetDateTime.now()
        val startDate = OffsetDateTime.of(today.year, 1, 1,0, 0, 0, 0, ZoneOffset.UTC)
        val endDate  = startDate
                .plusDays(MAX_DAYS_IN_REQUEST.toLong() - 1)
                .plusHours(MAX_HOUR.toLong())
                .plusMinutes(MAX_MINUTE_SECONDS.toLong())
                .plusSeconds(MAX_MINUTE_SECONDS.toLong())

        mockingClient.forTpp.mock {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "Requested record access is disabled by the practice",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun enabledWithRecords(patient: Patient, year: Int?, numberOfResults: Int?) {
        val daysLeft: Long
        var startDate: OffsetDateTime
        val finalEndDate: OffsetDateTime
        val today = OffsetDateTime.now()
        val resultsForAllButFinalCall = 0
        var resultsShown = TppConstants.DefaultTestResultsReturned

        if (numberOfResults != null) {
            resultsShown = numberOfResults
        }

        if (year != null) {
            startDate = OffsetDateTime.of(year, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC)
            finalEndDate  = OffsetDateTime.of(year,
                MAX_MONTH,
                MAX_DAY_INT,
                MAX_HOUR,
                MAX_DAYS_IN_REQUEST - 1,
                MAX_MINUTE_SECONDS,
                MAX_MINUTE_SECONDS,
                ZoneOffset.UTC)
        } else {
            startDate = OffsetDateTime.of(today.year, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC)
            finalEndDate = today
        }

        daysLeft = Duration.between(startDate, finalEndDate).toDays()

        var requestEndDate  = startDate
                .plusDays(MAX_DAYS_IN_REQUEST.toLong() - 1)
                .plusHours(MAX_HOUR.toLong())
                .plusMinutes(MAX_MINUTE_SECONDS.toLong())
                .plusSeconds(MAX_MINUTE_SECONDS.toLong())

        val testResultsCallsRequired = floor(daysLeft.toDouble() / MAX_DAYS_IN_REQUEST).toInt()

        (1..testResultsCallsRequired).forEach { _ ->
            mockingClient.forTpp.mock {
                myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, requestEndDate)
                        .respondWithSuccess(TestResultsData
                                .getMultipleTestResultsForTpp(resultsForAllButFinalCall))
            }

            startDate = startDate.plusDays(MAX_DAYS_IN_REQUEST.toLong())
            requestEndDate = requestEndDate.plusDays(MAX_DAYS_IN_REQUEST.toLong())
        }

        mockingClient.forTpp.mock {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, finalEndDate)
                .respondWithSuccess(TestResultsData
                    .getMultipleTestResultsForTpp(resultsShown))
        }
    }

    override fun errorRetrieving(patient: Patient) {
       val today = OffsetDateTime.now()

        val startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        val endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp.mock {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithServiceNotAvailableException()
        }
    }

    override fun noAccess(patient: Patient) {
        val today = OffsetDateTime.now()

        val startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        val endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp.mock {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "You don&apos;t have access to this online service. " +
                                    "You can request access to this service at Kainos GP Demo Unit by " +
                                    "clicking Manage Online Services in the Account section.",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun getExpectedTestResults(): List<TestResultItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }
}

