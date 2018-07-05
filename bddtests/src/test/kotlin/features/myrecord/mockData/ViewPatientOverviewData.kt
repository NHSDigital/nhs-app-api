package features.myrecord.mockData

import mocking.emis.models.*
import mocking.emis.testResults.TestResultMedicalRecord
import mocking.emis.testResults.TestResultResponse
import mocking.emis.testResults.TestResultResponseModel
import mocking.tpp.models.Item
import mocking.tpp.models.ViewPatientOverviewReply
import java.time.LocalDateTime

object ViewPatientOverviewData {

    fun getTppViewPatientOverviewData(): ViewPatientOverviewReply {
        return ViewPatientOverviewReply(
                drugs = mutableListOf(),
                currentRepeats  = mutableListOf(),
                pastRepeats  = mutableListOf()
        )
    }
}
