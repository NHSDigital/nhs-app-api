package mocking.tpp

import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.tpp.requestPatientRecord.TppRequestPatientRecordBuilder
import mocking.tpp.testResultDetail.TppTestResultDetailBuilder
import mocking.tpp.testResultsView.TppTestResultsViewBuilder
import mocking.tpp.viewPatientOverview.TppViewPatientOverviewBuilder
import worker.models.demographics.TppUserSession
import java.time.OffsetDateTime

class TppMappingBuilderMyRecord{
    fun testResultsViewRequest(tppUserSession: TppUserSession, startDate: OffsetDateTime, endDate: OffsetDateTime) =
            TppTestResultsViewBuilder(tppUserSession, startDate, endDate)

    fun testResultsDetailRequest(tppUserSession: TppUserSession, testResultId: String) =
            TppTestResultDetailBuilder(tppUserSession, testResultId)

    fun patientRecordRequest(tppUserSession: TppUserSession) = TppRequestPatientRecordBuilder(tppUserSession)

    fun viewPatientOverviewPost(tppUserSession: TppUserSession) = TppViewPatientOverviewBuilder(tppUserSession)

    fun patientSelectedPost(tppUserSession: TppUserSession) = TppPatientSelectedBuilder(tppUserSession)
}