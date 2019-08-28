package mocking.stubs.myMedicalRecord.emis

import mocking.MockingClient
import mocking.data.myrecord.DemographicsData
import mocking.emis.demographics.EmisDemographicsBuilder
import mocking.stubs.EmisStubsPatientFactory
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment
import models.Patient
import java.time.Duration

class DemographicsStubs (private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val demographicsDataLoader = DemographicsData.getEmisDemographicData(EmisStubsPatientFactory.goodPatientEMIS)
        val mapEMISConsultationsRequestStubs =
                InputResponse<Patient, EmisDemographicsBuilder>()
                        .addResponse(EmisStubsPatientFactory.goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(demographicsDataLoader)
                        }

                        .addResponse(EmisStubsPatientFactory.serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(EmisStubsPatientFactory.timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(demographicsDataLoader)
                                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
                        }

        mapEMISConsultationsRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(myRecord.demographicsRequest(scenario.forMatcher)) }
        }
    }
}
