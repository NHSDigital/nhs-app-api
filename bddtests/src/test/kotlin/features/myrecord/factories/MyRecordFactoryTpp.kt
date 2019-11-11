package features.myrecord.factories

import constants.ErrorResponseCodeTpp
import mocking.data.myrecord.TestResultsData
import mocking.data.myrecord.TppDcrData
import mocking.data.myrecord.ViewPatientOverviewData
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions
import mocking.tpp.models.Error
import models.Patient
import java.time.OffsetDateTime

private const val END_DATE = 60L
class MyRecordFactoryTpp: MyRecordFactory() {

    private val testResultsFactory by lazy {TestResultsFactoryTpp()}
    private val patientOverviewFactory by lazy {PatientOverviewFactoryTpp()}

    override fun disabled(patient: Patient) {
        mockingClient.forTpp {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "Requested record access is disabled by the practice",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {

        mockingClient.forTpp {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithSuccess(ViewPatientOverviewData.getTppViewPatientOverviewData())
        }

        mockingClient.forTpp {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppDcrData.getDefaultTppDcrData())
        }

        val startDate = OffsetDateTime.now()
        val endDate = startDate.minusDays(END_DATE)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithSuccess(TestResultsData.getDefaultTppTestResultsData())
        }
    }

    override fun enabledWithData(
            patient: Patient, myRecordModuleCounts: MyRecordModuleCounts, testResultOptions: TestResultOptions) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithAllRecords(patient: Patient) {
        mockingClient.forTpp {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithSuccess(patientOverviewFactory.getTppPatientOverviewData())
        }

        mockingClient.forTpp {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppDcrData.getMultipleDcrEventsForTpp())
        }

        testResultsFactory.enabledWithRecords(patient)
    }

    override fun respondWithForbidden(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun disabledForProxy(patient: Patient, actingOnBehalfOf: Patient) {
        throw NotImplementedError("Not implemented for this GP system")
    }
}