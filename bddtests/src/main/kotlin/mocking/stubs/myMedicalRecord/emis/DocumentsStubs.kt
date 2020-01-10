package mocking.stubs.myMedicalRecord.emis

import mocking.MockingClient
import mocking.data.myrecord.DocumentsData
import mocking.emis.documents.EmisDocumentsBuilder
import mocking.stubs.EmisStubsPatientFactory
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment
import models.Patient
import java.time.Duration

class DocumentsStubs (private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val documentDataLoader = DocumentsData.getDefaultDocumentsData(hasSize = false)
        val mapEMISDocumentsStubs =
                InputResponse<Patient, EmisDocumentsBuilder>()
                        .addResponse(EmisStubsPatientFactory.goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(documentDataLoader)
                        }

                        .addResponse(EmisStubsPatientFactory.serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(EmisStubsPatientFactory.timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(documentDataLoader)
                                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
                        }

        mapEMISDocumentsStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(myRecord.documentsRequest(scenario.forMatcher)) }
        }
    }
}
