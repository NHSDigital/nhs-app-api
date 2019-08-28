package mocking.stubs.myMedicalRecord.emis

import mocking.MockingClient
import mocking.data.myrecord.AllergiesData
import mocking.emis.allergies.EmisAllergiesBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class AllergiesStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val allergiesDataLoader = AllergiesData.getEmisAllergyRecordsWithDifferentDateParts()
        val mapEMISAllergiesRequestStubs =
                InputResponse<Patient, EmisAllergiesBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(allergiesDataLoader)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(allergiesDataLoader)
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISAllergiesRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(myRecord.allergiesRequest(scenario.forMatcher)) }
        }

    }
}