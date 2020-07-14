package mocking.stubs.myMedicalRecord.tpp

import mocking.MockingClient
import mocking.stubs.InputResponse
import mocking.stubs.TppStubsPatientFactory
import mocking.tpp.viewPatientOverview.TppViewPatientOverviewBuilder
import models.Patient
import mocking.stubs.myMedicalRecord.tpp.PatientSelectedTPP.Companion.goodPatientUserSession
import mocking.tpp.data.PatientOverviewData

class PatientOverviewTpp(private val mockingClient: MockingClient) {
    fun generateTPPStubs() {
        val patientOverviewData = PatientOverviewData().patientOverviewData
        val mapTPPAllergiesRequestStubs =
                InputResponse<Patient, TppViewPatientOverviewBuilder>()
                        .addResponse(TppStubsPatientFactory.goodPatientTPP) { builder
                            ->
                            builder.respondWithSuccess(patientOverviewData)
                        }

        mapTPPAllergiesRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forTpp.mock { scenario.getResponse(TppViewPatientOverviewBuilder(goodPatientUserSession)) }
        }

    }
}
