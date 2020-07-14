package mocking.stubs.myMedicalRecord.tpp

import mocking.MockingClient
import mocking.stubs.InputResponse
import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP
import mocking.tpp.data.PatientSelectedDataTpp
import worker.models.demographics.TppUserSession

class PatientSelectedTPP(private val mockingClient: MockingClient)
{companion object {
    val goodPatientUserSession = goodPatientTPP.tppUserSession!!
}

    fun generateTPPStubs() {
        val selectedPatient = PatientSelectedDataTpp().selectedPatient
        val mapTPPAllergiesRequestStubs =
                InputResponse<TppUserSession, TppPatientSelectedBuilder>()
                        .addResponse(goodPatientUserSession) { builder
                            ->
                            builder.respondWithSuccess(selectedPatient)
                        }

        mapTPPAllergiesRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forTpp.mock { scenario.getResponse(TppPatientSelectedBuilder(scenario.forMatcher)) }
        }

    }
}
