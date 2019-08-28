package mocking.stubs.myMedicalRecord.tpp

import mocking.MockingClient
import mocking.stubs.InputResponse
import mocking.stubs.TppStubsPatientFactory
import mocking.stubs.myMedicalRecord.tpp.PatientSelectedTPP.Companion.goodPatientUserSession
import mocking.tpp.models.RequestPatientRecordReply
import mocking.tpp.requestPatientRecord.TppRequestPatientRecordBuilder
import models.Patient

class RequestPatientRecordTpp(private val mockingClient: MockingClient) {
    fun generateTPPStubs() {
        val allergiesDataLoader = RequestPatientRecordReply()
        val mapTPPAllergiesRequestStubs =
                InputResponse<Patient, TppRequestPatientRecordBuilder>()
                        .addResponse(TppStubsPatientFactory.goodPatientTPP) { builder
                            ->
                            builder.respondWithSuccess(allergiesDataLoader)
                        }

        mapTPPAllergiesRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forTpp { scenario.getResponse(TppRequestPatientRecordBuilder(goodPatientUserSession)) }
        }

    }
}