package mocking.stubs.myMedicalRecord.emis

import mocking.MockingClient
import mocking.data.myrecord.ConsultationsData
import mocking.emis.consultations.EmisConsultationsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class ConsultationsStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val consultationsDataLoader = ConsultationsData.getMultipleConsultationRecords()
        val mapEMISConsultationsRequestStubs =
                InputResponse<Patient, EmisConsultationsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(consultationsDataLoader)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(consultationsDataLoader)
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISConsultationsRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis.mock { scenario.getResponse(myRecord.consultationsRequest(scenario.forMatcher)) }
        }
    }
}
