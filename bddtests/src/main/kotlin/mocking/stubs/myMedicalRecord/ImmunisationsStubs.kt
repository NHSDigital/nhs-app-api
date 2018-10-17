package mocking.stubs.myMedicalRecord

import mocking.MockingClient
import mocking.data.myrecord.ImmunisationsData
import mocking.emis.immunisations.EmisImmunisationsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.StubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class ImmunisationsStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val ImunisationDataLoader = ImmunisationsData.getImmunisationsData()
        val mapEMISImmunisationResultsStubs =
                InputResponse<Patient, EmisImmunisationsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(ImunisationDataLoader)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(ImunisationDataLoader)
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISImmunisationResultsStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(myRecord.immunisationsRequest(scenario.forMatcher)) }
        }

    }
}